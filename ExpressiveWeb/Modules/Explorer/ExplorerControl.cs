// *********************************************************
// 
// ExpressiveWeb ExplorerControl.cs
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
using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using ExpressiveWeb.Core.Project;

namespace ExpressiveWeb.Modules.Explorer;

public class ExplorerControl : TemplatedControl
{
    private ObservableCollection<ProjectItem> _items = new();
    private TreeView? _treeView;

    public event EventHandler<ProjectItem>? ItemDoubleClicked;

    public ObservableCollection<ProjectItem> Items
    {
        get
        {
            return _items;
        }
        set
        {
            _items = value;
            if (_treeView != null)
            {
                _treeView.ItemsSource = _items;
            }
        }
    }

    public ProjectItem SelectedItem
    {
        get;
        private set;
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        _treeView = e.NameScope.Find<TreeView>("PART_TreeView");

        _treeView?.AddHandler(PointerPressedEvent, TreeView_Click, RoutingStrategies.Tunnel);
        _treeView?.AddHandler(PointerPressedEvent, TreeView_DblClick, RoutingStrategies.Tunnel);
    }

    private void TreeView_Click(object? sender, PointerPressedEventArgs e)
    {
        if (e.ClickCount != 1)
        {
            return;
        }

        PointerPoint pointerPoint = e.GetCurrentPoint(sender as Control);

        if (pointerPoint.Properties.IsLeftButtonPressed && e.Source is Control c && c.DataContext is ProjectItem it)
        {
            SelectedItem = it;
        }
    }

    private void TreeView_DblClick(object? sender, PointerPressedEventArgs e)
    {
        if (e.ClickCount != 2)
        {
            return;
        }

        PointerPoint pointerPoint = e.GetCurrentPoint(sender as Control);

        if (ItemDoubleClicked != null && pointerPoint.Properties.IsLeftButtonPressed && e.Source is Control c && c.DataContext is ProjectItem it)
        {
            ItemDoubleClicked.Invoke(this, it);
            e.Handled = true;
        }
    }
}