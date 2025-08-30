// *********************************************************
// 
// ExpressiveWeb.UI.Internal EWMainMenuBar.cs
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

using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using ExpressiveWeb.Core;
using ExpressiveWeb.Core.ApplicationCommands;
using Microsoft.Extensions.DependencyInjection;

namespace ExpressiveWeb.Presentation.Menus;

public class EWMainMenuBar : TemplatedControl
{
    private Menu _mainMenu;
    private IApplicationCommandsService _applicationCommandsService;

    public EWMainMenuBar()
    {
        _applicationCommandsService = AppServices.ServicesFactory!.GetService<IApplicationCommandsService>()!;
    }

    public void AppendMenu(string header, List<ApplicationCommandBase> commands)
    {
        EWMenuItem menuItem = new()
        {
            Header = header
        };

        foreach (ApplicationCommandBase cmd in commands)
        {
            object childMenuItem = null;

            if (menuItem is EWMenuItem cx1)
            {
                childMenuItem = CommonMenuHelper.BuildMenuItem(this, cmd);
                cx1.Items.Add(childMenuItem);
            }
            // else if (menu is CxRibbonTabItem cx2)
            // {
            //     menuItem = BuildToolbarButton(cmd);
            //     cx2.Items.Add(menuItem);
            // }

            _applicationCommandsService.RegisterCommand(cmd);

            // Si la commande a des sous-commandes lors de la création, on register aussi les sous-commandes
            // attention, avec ce principe on ne gère qu'un seul niveau d'enfants
            if (cmd.HasSubCommands && childMenuItem is EWMenuItem menuItem2)
            {
                foreach (ApplicationCommandBase subCommand in cmd.SubCommands!)
                {
                    object menuItem3 = CommonMenuHelper.BuildMenuItem(this, subCommand);
                    menuItem2.Items.Add(menuItem3);
                    _applicationCommandsService.RegisterCommand(subCommand);

                    if (subCommand.SubCommands != null)
                    {
                        foreach (ApplicationCommandBase subCommand2 in subCommand.SubCommands)
                        {
                            ((EWMenuItem) menuItem3).Items.Add(CommonMenuHelper.BuildMenuItem(this, subCommand2));
                            _applicationCommandsService.RegisterCommand(subCommand2);
                        }
                    }
                }
            }
        }

        _mainMenu.Items.Add(menuItem);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        _mainMenu = e.NameScope.Find<Menu>("PART_MainMenu");
    }
}