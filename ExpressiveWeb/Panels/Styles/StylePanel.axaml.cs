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
using Avalonia.Controls.Presenters;
using Avalonia.Input;
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

    private HtmlElement? _currentHtmlElement;
    private bool _ignoreSelectionChanged;

    public StylePanel()
    {
        InitializeComponent();
        _styleService = AppServices.ServicesFactory.GetService<IStyleService>();

        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    private void ApplicationSharedEventsOnSelectedElementChanged(object? sender, HtmlElement? e)
    {
        _ignoreSelectionChanged = true;

        if (_currentHtmlElement == e)
        {
            return;
        }

        CbStylesList.IsEnabled = e != null && e.KitComponent != null;

        _currentHtmlElement = e;

        if (e != null && e.KitComponent != null)
        {
            BuildStylesCombo(e);
        }

        if (e == null || CbStylesList.SelectedItem is StyleItemModel {Source: StyleItemModel.StyleItemSource.Kit})
        {
            PgStyle.IsEnabled = false;
            PgStyle.DataContext = null;
        }
        else
        {
            PgStyle.IsEnabled = true;
            CssStyle st = _styleService.ParseStyleAttribute(_currentHtmlElement?.GetAttribute("style") ?? string.Empty);
            PgStyle.DataContext = st;
        }

        _ignoreSelectionChanged = false;
    }

    private void BuildStylesCombo(HtmlElement element)
    {
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
                    CssClass = variant.CssClass
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
    }

    private void CbStylesList_OnDropDownClosed(object? sender, EventArgs e)
    {
        if (AppState.Instance.AppWindow.ApplicationWorkspaceControl.IsCurrentDocumentOfType(out EditorView? editorWorkspace))
        {
            editorWorkspace!.Editor.EndCssClassPreview();
        }
    }

    private void CbStylesList_OnPointerExited(object? sender, PointerEventArgs e)
    {
        if (AppState.Instance.AppWindow.ApplicationWorkspaceControl.IsCurrentDocumentOfType(out EditorView? editorWorkspace))
        {
            editorWorkspace!.Editor.EndCssClassPreview();
        }
    }

    private void CbStylesList_OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (e.Source is ContentPresenter p && p.DataContext is StyleItemModel styleItemModel && AppState.Instance.AppWindow.ApplicationWorkspaceControl.IsCurrentDocumentOfType(out EditorView? editorWorkspace))
        {
            string currentElementStyle = ComputeElementCssClassName(styleItemModel);
            editorWorkspace!.Editor.StartCssClassPreview(currentElementStyle);
        }
    }

    private void CbStylesList_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_ignoreSelectionChanged)
        {
            return;
        }

        if (CbStylesList.SelectedItem is StyleItemModel styleItemModel)
        {
            string currentElementStyle = ComputeElementCssClassName(styleItemModel);

            if (AppState.Instance.AppWindow.ApplicationWorkspaceControl.IsCurrentDocumentOfType(out EditorView? editorWorkspace))
            {
                editorWorkspace!.Editor.SetElementCssClass(currentElementStyle);
            }
        }
    }

    private string ComputeElementCssClassName(StyleItemModel styleItemModel)
    {
        string currentElementStyle = _currentHtmlElement!.CssClass;

        foreach (StyleItemModel styleItem in CbStylesList.Items.OfType<StyleItemModel>())
        {
            if (styleItem.Source == StyleItemModel.StyleItemSource.Kit && !string.IsNullOrEmpty(styleItem.CssClass))
            {
                currentElementStyle = currentElementStyle.Replace(styleItem.CssClass, string.Empty, StringComparison.Ordinal);
            }
        }

        currentElementStyle = currentElementStyle.Trim();

        if (styleItemModel.Source == StyleItemModel.StyleItemSource.Kit)
        {
            currentElementStyle = string.Concat(currentElementStyle, " ", styleItemModel.CssClass);
        }

        return currentElementStyle;
    }

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        ApplicationSharedEvents.SelectedElementChanged += ApplicationSharedEventsOnSelectedElementChanged;
    }

    private void OnUnloaded(object? sender, RoutedEventArgs e)
    {
        ApplicationSharedEvents.SelectedElementChanged -= ApplicationSharedEventsOnSelectedElementChanged;
    }

    private void PgStyle_OnValueApplied(object? sender, EventArgs eventArgs)
    {
        if (AppState.Instance.AppWindow.ApplicationWorkspaceControl.IsCurrentDocumentOfType(out EditorView? editorWorkspace))
        {
            editorWorkspace!.Editor.SetElementStyleAttribute(PgStyle.DataContext as CssStyle);
        }
    }
}