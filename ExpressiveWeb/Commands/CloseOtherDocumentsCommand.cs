// *********************************************************
// 
// ExpressiveWeb CloseOtherDocumentsCommand.cs
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

using ExpressiveWeb.Core.ApplicationCommands;
using ExpressiveWeb.Localization;

namespace ExpressiveWeb.Commands;

public class CloseOtherDocumentsCommand : ApplicationCommandBase
{
    public CloseOtherDocumentsCommand()
    {
        IsEnabled = true;
    }

    public override string CommandName
    {
        get
        {
            return "CloseOtherDocuments";
        }
    }

    public override string Title
    {
        get
        {
            return Resources.ViewCloseOtherDocuments;
        }
    }

    public override void Execute()
    {
        AppState.Instance.AppWindow.ApplicationWorkspaceControl.CloseOtherDocuments();
    }
}