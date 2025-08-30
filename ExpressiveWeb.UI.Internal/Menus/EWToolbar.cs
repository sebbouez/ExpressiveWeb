// *********************************************************
// 
// ExpressiveWeb.UI.Shell EWToolbar.cs
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

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;

namespace ExpressiveWeb.Presentation.Menus;

public class EWToolbar : TemplatedControl
{
    private ItemsControl _mainItemsControl;
    private ToggleButton _overflowButton;
    private ItemsControl _overflowItemsControl;
    private Flyout? _overflowPopup;

    public EWToolbar()
    {
        Loaded += OnLoaded;
    }

    public List<object> Items
    {
        get;
        set;
    } = new();

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        _mainItemsControl = e.NameScope.Find<ItemsControl>("PART_MainItemsControl");
        _overflowItemsControl = e.NameScope.Find<ItemsControl>("PART_OverflowItemsControl");
        _overflowButton = e.NameScope.Find<ToggleButton>("PART_OverflowButton");
        e.NameScope.Find<Popup>("PART_OverflowPopup");

        if (_overflowButton != null)
        {
            _overflowPopup = (FlyoutBase.GetAttachedFlyout(_overflowButton) as Flyout);

            _overflowPopup!.Closed += OnClosed;
            _overflowPopup.Placement = PlacementMode.Custom;
            _overflowPopup.CustomPopupPlacementCallback = (p) =>
            {
                p.Offset = new Point(-p.PopupSize.Width / 2 + p.AnchorRectangle.Width / 2, p.PopupSize.Height / 2 + p.AnchorRectangle.Height / 2);
            };

            _overflowButton.Click += OverflowButtonOnClick;
        }
    }

    private void OnClosed(object? sender, EventArgs e)
    {
        _overflowButton.IsChecked = false;
    }

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        InvalidateMeasure();
    }

    private void OverflowButtonOnClick(object? sender, RoutedEventArgs e)
    {
        if (_overflowButton.IsChecked!.Value)
        {
            _overflowPopup!.ShowAt(_overflowButton);
        }
        else
        {
            _overflowPopup!.Hide();
        }
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        ReorganizeItems(availableSize);
        return base.MeasureOverride(availableSize);
    }

    private void ReorganizeItems(Size availableSize)
    {
        _mainItemsControl.Items.Clear();
        _overflowItemsControl.Items.Clear();

        double maxWidth = availableSize.Width - 30;
        double usedWidth = 0;

        foreach (Control control in Items.OfType<Control>())
        {
            control.Measure(new Size(double.PositiveInfinity, 30));
            double w = control.DesiredSize.Width;

            if (control.Parent is ItemsControl p)
            {
                p.Items.Remove(control);
            }

            usedWidth += w;

            if (usedWidth < maxWidth)
            {
                _mainItemsControl.Items.Add(control);
            }
            else
            {
                _overflowItemsControl.Items.Add(control);
            }
        }

        _overflowButton.IsVisible = _overflowItemsControl.Items.Count > 0;
    }
}