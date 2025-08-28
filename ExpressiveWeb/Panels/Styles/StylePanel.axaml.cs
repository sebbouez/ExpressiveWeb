// *********************************************************
// 
// ExpressiveWeb StylePanel.axaml.cs
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
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using ExpressiveWeb.Core;
using ExpressiveWeb.Core.Kit;
using ExpressiveWeb.Core.Style;
using ExpressiveWeb.Designer.Models;
using ExpressiveWeb.Modules.EditorView;
using Microsoft.Extensions.DependencyInjection;

namespace ExpressiveWeb.Panels.Styles;

public partial class StylePanel : UserControl
{
    private readonly IStyleService _styleService;
    private bool _ignoreSelectionChanged;

    private HtmlElement? _currentHtmlElement;

    public StylePanel()
    {
        InitializeComponent();
        _styleService = AppServices.ServicesFactory.GetService<IStyleService>();
        
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    private void ApplicationSharedEventsOnSelectedElementChanged(object? sender, HtmlElement? e)
    {
        CbStylesList.IsEnabled = e != null && e.KitComponent != null;

        _currentHtmlElement = e;

        if (e != null && e.KitComponent != null)
        {
            BuildStylesCombo(e);
        }
    }

    private void BuildStylesCombo(HtmlElement element)
    {
        _ignoreSelectionChanged = true;

        List<StyleItemModel> items = new();

        StyleItemModel defaultItem = new()
        {
            Name = "Default",
            CssClass = string.Empty
        };
        items.Add(defaultItem);

        StyleItemModel selectedItem = defaultItem;

        if (element.KitComponent != null)
        {
            StyleItemModel separator = new()
            {
                IsSeparator = true
            };

            items.Add(separator);

            foreach (ComponentVariant variant in element.KitComponent.Variants)
            {
                StyleItemModel variantItem = new()
                {
                    Name = variant.Name,
                    Source = StyleItemModel.StyleItemSource.Kit,
                    SecondaryText = "Kit",
                    CssClass = variant.CssClass,
                };
                items.Add(variantItem);

                if (!string.IsNullOrEmpty(variant.CssClass) && _currentHtmlElement!.HasCssClass(variant.CssClass))
                {
                    selectedItem = variantItem;
                }
            }
        }

        CbStylesList.ItemsSource = items;
        CbStylesList.SelectedItem = selectedItem;

        _ignoreSelectionChanged = false;
    }

    private void CbStylesList_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_ignoreSelectionChanged)
        {
            return;
        }

        string currentElementStyle = _currentHtmlElement!.CssClass;

        foreach (StyleItemModel styleItem in CbStylesList.Items.OfType<StyleItemModel>())
        {
            if (styleItem.Source == StyleItemModel.StyleItemSource.Kit && !string.IsNullOrEmpty(styleItem.CssClass))
            {
                currentElementStyle = currentElementStyle.Replace(styleItem.CssClass, string.Empty, StringComparison.Ordinal);
            }
        }

        currentElementStyle = currentElementStyle.Trim();

        if (CbStylesList.SelectedItem is StyleItemModel selectedItem && selectedItem.Source == StyleItemModel.StyleItemSource.Kit)
        {
            currentElementStyle = string.Concat(currentElementStyle, " ", selectedItem.CssClass);
        }

        if (AppState.Instance.AppWindow.ApplicationWorkspaceControl.IsCurrentDocumentOfType(out EditorView? editorWorkspace))
        {
            editorWorkspace!.Editor.SetElementCssClass(currentElementStyle);
        }
    }

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        ApplicationSharedEvents.SelectedElementChanged += ApplicationSharedEventsOnSelectedElementChanged;
    }

    private void OnUnloaded(object? sender, RoutedEventArgs e)
    {
        ApplicationSharedEvents.SelectedElementChanged -= ApplicationSharedEventsOnSelectedElementChanged;
    }
}