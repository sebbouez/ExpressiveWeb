// *********************************************************
// 
// ExpressiveWeb OpenProjectCommand.cs
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

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using ExpressiveWeb.Core.ApplicationCommands;
using ExpressiveWeb.Core.Html;
using ExpressiveWeb.Core.Project;
using ExpressiveWeb.Localization;
using ExpressiveWeb.Modules.Toolbox;
using ExpressiveWeb.Presentation.MessageBox;

namespace ExpressiveWeb.Commands;

public class OpenProjectCommand : ApplicationCommandBase
{
    public OpenProjectCommand()
    {
        IsEnabled = true;
    }

    public override string CommandName
    {
        get
        {
            return "OpenProject";
        }
    }

    public override string IconResourceName
    {
        get
        {
            return "IconOpen";
        }
    }

    public override string Title
    {
        get
        {
            return Resources.OpenProject;
        }
    }

    internal void DoOpenProject(string filePath)
    {
        AppState.Instance.AppWindow.SetStatusMessage("Loading project...");


        ProjectLoader loader = new();
        _ = loader.Load(filePath).ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                _ = EWMessageBox.Show(new MessageBoxData
                {
                    Owner = AppState.Instance.AppWindow,
                    Buttons = MessageBoxButtons.Ok,
                    Message = "Error loading project",
                    Title = "Error"
                });
                return;
            }

            AppState.Instance.CurrentProject = task.Result;
            ObservableCollection<ToolboxItem> items = new();

            foreach (HtmlElementDeclaration elementDeclaration in AppState.Instance.CurrentProject!.Kit.KnownElements)
            {
                items.Add(new ToolboxItem
                {
                    Name = elementDeclaration.Name
                });
            }

            ApplicationSharedEvents.InvokeProjectLoaded(AppState.Instance.CurrentProject);

            AppState.Instance.AppWindow.SetStatusMessage(Resources.StatusReady);
        }, TaskScheduler.FromCurrentSynchronizationContext());
    }

    public override async void Execute()
    {
        FilePickerFileType projectTypes = new("Project Type")
        {
            Patterns = new[] {"*.ewproj"}
        };

        FilePickerOpenOptions options = new()
        {
            Title = Resources.OpenProject,
            AllowMultiple = false,
            FileTypeFilter =
            [
                projectTypes
            ]
        };


        IReadOnlyList<IStorageFile> r = await AppState.Instance.AppWindow.StorageProvider.OpenFilePickerAsync(options);

        if (!r.Any())
        {
            return;
        }

        IStorageFile file = r.First();

        DoOpenProject(file.Path.LocalPath);
    }
}