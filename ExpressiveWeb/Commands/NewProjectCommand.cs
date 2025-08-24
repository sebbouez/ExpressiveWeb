// *********************************************************
// 
// ExpressiveWeb NewProjectCommand.cs
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
using ExpressiveWeb.Modules.NewProject;

namespace ExpressiveWeb.Commands;

public class NewProjectCommand : ApplicationCommandBase
{
    public NewProjectCommand()
    {
        IsEnabled = true;
    }

    public override string CommandName
    {
        get
        {
            return "NewProject";
        }
    }

    public override string Title
    {
        get
        {
            return Resources.MenuNewProject;
        }
    }

    public override async void Execute()
    {
        CreateProjectDialog dlg = new();
        string? r = await dlg.ShowDialog<string?>(AppState.Instance.AppWindow);

        if (!string.IsNullOrEmpty(r))
        {
            OpenProjectCommand cmd = new();
            cmd.DoOpenProject(r);
        }
    }
}