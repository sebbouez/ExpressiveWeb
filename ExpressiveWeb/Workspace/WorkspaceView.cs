// *********************************************************
// 
// ExpressiveWeb WorkspaceView.cs
// Copyright (c) Sébastien Bouez. All rights reserved.
// THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 
// *********************************************************

using System;
using System.Threading.Tasks;
using Avalonia.Threading;
using Dock.Model.Avalonia.Controls;
using ExpressiveWeb.Presentation.MessageBox;

namespace ExpressiveWeb.Workspace;

public class WorkspaceView : Document
{
    private bool? _canClose;

    internal void AttachContent(IWorkspaceTabViewBase content)
    {
        Content = content;
        content.IsDirtyStateChanged += ContentOnIsDirtyStateChanged;
    }

    private void ContentOnIsDirtyStateChanged(object? sender, bool e)
    {
        if (e)
        {
            _canClose = null;
        }

        Dispatcher.UIThread.Post(() =>
        {
            IsModified = e;
        });
    }

    internal IWorkspaceTabViewBase GetContent()
    {
        return (IWorkspaceTabViewBase) Content;
    }

    private async Task ManageCloseAsync(Action confirmationCallback)
    {
        MessageBoxResult msgResult = await EWMessageBox.Show(new MessageBoxData
        {
            Owner = AppState.Instance.AppWindow,
            Buttons = MessageBoxButtons.YesNoCancel,
            Title = "Save before closing?",
            Message = "Do you want to save the changes before closing?"
        });

        switch (msgResult.SelectedButton)
        {
            case MessageBoxResultButton.Cancel:
                _canClose = false;
                return;
            case MessageBoxResultButton.Yes:
                await GetContent().SaveAsync();
                _canClose = true;
                confirmationCallback();
                break;
            case MessageBoxResultButton.No:
                _canClose = true;
                confirmationCallback();
                break;
        }
    }

    internal bool QueryCanClose(Action confirmationCallback)
    {
        if (GetContent().CanClose() || _canClose == true)
        {
            return true;
        }

        _ = ManageCloseAsync(confirmationCallback);
        return false;
    }
}