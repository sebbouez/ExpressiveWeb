// *********************************************************
// 
// ExpressiveWeb NewFolderCommand.cs
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
using System.IO;
using System.Threading.Tasks;
using ExpressiveWeb.Core;
using ExpressiveWeb.Core.ApplicationCommands;
using ExpressiveWeb.Core.Log;
using ExpressiveWeb.Core.Project;
using ExpressiveWeb.Modules.Explorer;
using ExpressiveWeb.Presentation.MessageBox;
using Microsoft.Extensions.DependencyInjection;

namespace ExpressiveWeb.Commands;

public class NewFolderCommand : ApplicationCommandBase
{
    public override string CommandName
    {
        get
        {
            return "NewFolder";
        }
    }

    public override string Title
    {
        get
        {
            return Localization.Resources.NewFolder;
        }
    }

    public override void Execute()
    {
        EWInputBox.Show(new InputBoxData()
        {
            Owner = AppState.Instance.AppWindow,
            Message = "Enter the folder name",
            Title = "New Folder"
        }).ContinueWith(task =>
        {
            if (string.IsNullOrEmpty(task.Result))
            {
                return;
            }

            AppState.Instance.AppWindow.ApplicationWorkspaceControl.GetPanel<ExplorerControl>(out ExplorerControl? explorerControl);

            string prjectRootPath = AppState.Instance.CurrentProject!.RootPath;
            string folderPath = Path.Combine(prjectRootPath, task.Result);

            if (explorerControl != null && explorerControl.SelectedItem is ProjectItem item
                                        && item.ItemType == ProjectItemType.Folder
                                        && !string.IsNullOrEmpty(item.Path))
            {
                folderPath = Path.Combine(item.Path, task.Result);
            }

            try
            {
                Directory.CreateDirectory(folderPath);

                // TODO it would be better to refresh the folder only, not the full tree
                AppState.Instance.CurrentProject.RefreshItems();
            }
            catch (Exception ex)
            {
                AppServices.ServicesFactory!.GetService<ILogService>()?.Error(ex);

                _ = EWMessageBox.Show(new MessageBoxData()
                {
                    Owner = AppState.Instance.AppWindow,
                    Message = "Unable to create folder",
                    Title = "Error",
                    Buttons = MessageBoxButtons.Ok,
                    Image = MessageBoxImage.Error
                });
            }
        }, TaskScheduler.FromCurrentSynchronizationContext());
    }
}