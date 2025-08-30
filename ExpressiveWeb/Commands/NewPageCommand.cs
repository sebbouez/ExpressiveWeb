// *********************************************************
// 
// ExpressiveWeb NewPageCommand.cs
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

using System.Threading.Tasks;
using ExpressiveWeb.Core;
using ExpressiveWeb.Core.ApplicationCommands;
using ExpressiveWeb.Core.Kit;
using ExpressiveWeb.Localization;
using ExpressiveWeb.Modules.NewPage;
using Microsoft.Extensions.DependencyInjection;

namespace ExpressiveWeb.Commands;

public class NewPageCommand : ApplicationCommandBase
{
    public override string CommandName
    {
        get
        {
            return "NewPage";
        }
    }

    public override string Title
    {
        get
        {
            return Resources.NewPage;
        }
    }

    public override string IconResourceName
    {
        get
        {
            return "IconAddPage";
        }
    }

    public override async void Execute()
    {
        CreatePageDialog dlg = new();
        bool r = await dlg.ShowDialog<bool>(AppState.Instance.AppWindow);

        if (!r)
        {
            return;
        }

        string filePath = dlg.FilePath;
        KitPageTemplate selectedTemplate = dlg.SelectedTemplate;

        using VariableDurationActionHandler handler = new(200);
        _ = handler.Run(async void () =>
        {
            AppServices.ServicesFactory!.GetService<IKitService>()!.CopyTemplateToFile(AppState.Instance.CurrentProject!.Kit.Name, selectedTemplate, filePath);
        }).ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                return;
            }

            // TODO a bit brutal, it would be better to refresh only what has changed
            AppState.Instance.CurrentProject!.RefreshItems();
        }, TaskScheduler.FromCurrentSynchronizationContext());
        ;
    }
}