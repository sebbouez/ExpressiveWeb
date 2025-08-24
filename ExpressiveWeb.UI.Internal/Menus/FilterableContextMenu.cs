// *********************************************************
// 
// ExpressiveWeb.UI.Internal FilterableContextMenu.cs
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
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using ExpressiveWeb.Core;
using ExpressiveWeb.Core.ApplicationCommands;
using ExpressiveWeb.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace ExpressiveWeb.Presentation.Menus;

public class FilterableContextMenu : Avalonia.Controls.ContextMenu
{
    private readonly IApplicationCommandsService _applicationCommandsService;
    private TextBox _filterTextBox;

    private List<object> _items = new();
    private ItemsPresenter _itemsPanel;

    public FilterableContextMenu()
    {
        _applicationCommandsService = AppServices.ServicesFactory!.GetService<IApplicationCommandsService>()!;
        Opened += OnOpened;
    }

    private void OnOpened(object? sender, RoutedEventArgs e)
    {
        _filterTextBox.Text = string.Empty;
        ResetDefaultItems();
    }

    public void AppendCommand<T>() where T : ApplicationCommandBase
    {
        List<object> itemsToSet = new();

        T? foundCommand = _applicationCommandsService.GetCommand<T>();
        if (foundCommand == null)
        {
            throw new NullReferenceException("Unknown command: " + typeof(T).Name);
        }

        object menuItem = CommonMenuHelper.BuildMenuItem(this, foundCommand!, true);
        itemsToSet.Add(menuItem);

        // Si la commande a des sous-commandes lors de la création, on register aussi les sous-commandes
        // attention, avec ce principe on ne gère qu'un seul niveau d'enfants
        // if (command.HasSubCommands && menuItem is MenuItem menuItem2)
        // {
        //     foreach (ApplicationCommandBase subCommand in command.SubCommands!)
        //     {
        //         AllCommandsCache.Add(subCommand);
        //         menuItem2.Items.Add(BuildMenuItem(subCommand, true));
        //     }
        // }

        _items.AddRange(itemsToSet);
    }

    private void ResetDefaultItems()
    {
        Items.Clear();
        foreach (object item in _items)
        {
            Items.Add(item);
        }
    }

    private void Handler(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Down && Items.Count > 0)
        {
            e.Handled = true;
        }

        if (string.IsNullOrEmpty(_filterTextBox.Text))
        {
            ResetDefaultItems();
            return;
        }

        Dispatcher.UIThread.QueueAction("filtercontextmenu_type", () =>
        {
            SearchCommandsByName(_filterTextBox.Text).ContinueWith(t =>
            {
                Items.Clear();
                foreach (object item in t.Result)
                {
                    Items.Add(item);
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());

            e.Handled = true;
        });
    }

    private async Task<List<object>> SearchCommandsByName(string searchText)
    {
        List<object> itemsToSet = new();

        IEnumerable<ApplicationCommandBase> foundCommands = Enumerable.Empty<ApplicationCommandBase>();
        await Task.Run(() =>
        {
            foundCommands = _applicationCommandsService.RegisteredCommands.Where(x =>
                x.Title.Contains(searchText, StringComparison.InvariantCultureIgnoreCase)).Take(8);
        });

        foreach (ApplicationCommandBase? command in foundCommands)
        {
            object menuItem = CommonMenuHelper.BuildMenuItem(this, command!, true);
            itemsToSet.Add(menuItem);

            //CommandsInputsCache.Add(new KeyValuePair<Type, object>(command!.GetType(), menuItem));

            // if (command.HasSubCommands && menuItem is CxMenuItem menuItem2)
            //     foreach (ApplicationCommandBase subCommand in command.SubCommands!)
            //         menuItem2.Items.Add(BuildMenuItem(subCommand, true));
        }

        return itemsToSet;
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        _filterTextBox = e.NameScope.Find<TextBox>("PART_FilterTextBox");
        _itemsPanel = e.NameScope.Find<ItemsPresenter>("PART_ItemsPresenter");

        _filterTextBox!.Watermark = Localization.Resources.WatermarkFilterCommands;
        _filterTextBox!.AddHandler(KeyUpEvent, Handler);
    }
}