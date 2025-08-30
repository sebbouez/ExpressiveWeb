// *********************************************************
// 
// ExpressiveWeb MoveElementFirstCommand.cs
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
using ExpressiveWeb.Designer.Commands;
using ExpressiveWeb.Localization;
using ExpressiveWeb.Modules.EditorView;

namespace ExpressiveWeb.Commands;

public class MoveElementFirstCommand : ApplicationCommandBase
{
    public override string CommandName
    {
        get
        {
            return "MoveElementFirst";
        }
    }

    public override string Title
    {
        get
        {
            return Resources.MoveElementFirst;
        }
    }

    public override string IconResourceName
    {
        get
        {
            return "IconMoveFirst";
        }
    }

    public override void Execute()
    {
        if (AppState.Instance.AppWindow.ApplicationWorkspaceControl.IsCurrentDocumentOfType(out EditorView? editorWorkspace))
        {
            editorWorkspace!.Editor.ChangeElementIndex(MoveRelativePosition.First);
        }
    }
}