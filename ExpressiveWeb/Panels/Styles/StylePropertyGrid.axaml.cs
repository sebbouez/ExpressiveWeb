// *********************************************************
// 
// ExpressiveWeb.UI.Internal StylePropertyGrid.axaml.cs
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
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using ExpressiveWeb.Core.Style;

namespace ExpressiveWeb.Panels.Styles;

public partial class StylePropertyGrid : UserControl
{
    public enum LinkedProperties
    {
        None,
        LeftRight,
        TopBottom,
        All
    }

    private bool _ignoreEvents;

    public StylePropertyGrid()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
    }

    private void ApplyPadding(object? sender)
    {
        if (ReferenceEquals(sender, TbPaddingLeft) && BtnLockLeftRight.IsChecked!.Value)
        {
            TbPaddingRight.Value = TbPaddingLeft.Value;
        }

        if (ReferenceEquals(sender, TbPaddingTop) && BtnLockTopBottom.IsChecked!.Value)
        {
            TbPaddingBottom.Value = TbPaddingTop.Value;
        }


        if (ReferenceEquals(sender, TbMarginLeft) && BtnLockMarginLeftRight.IsChecked!.Value)
        {
            TbMarginRight.Value = TbMarginLeft.Value;
        }

        if (ReferenceEquals(sender, TbMarginTop) && BtnLockMarginTopBottom.IsChecked!.Value)
        {
            TbMarginBottom.Value = TbMarginTop.Value;
        }

        ValueApplied?.Invoke(this, EventArgs.Empty);
    }

    private void ColorView_OnColorChanged(object? sender, ColorChangedEventArgs e)
    {
        if (_ignoreEvents)
        {
            return;
        }

        if (DataContext is CssStyle style)
        {
            style.BackgroundColor = e.NewColor;
            ValueApplied?.Invoke(this, EventArgs.Empty);
        }
    }

    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        _ignoreEvents = true;

        if (DataContext is CssStyle style)
        {
            CpBackground.Color = style.BackgroundColor ?? Colors.Transparent;

            CpBorder.Color = style.BorderColor ?? Colors.Transparent;
            TbBorderAllWidth.Value = style.BorderWidth;
            TbBorderAllCornerRadius.Value = style.CornerRadius;
            InitComboBox(CbBorderAllStyle, style.BorderStyle);
        }

        _ignoreEvents = false;
    }

    private void InitComboBox(ComboBox comboBox, string? value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            comboBox.SelectedItem = comboBox.Items.OfType<ComboBoxItem>().FirstOrDefault(x => x.Tag != null && x.Tag.ToString()!.Equals(value, StringComparison.InvariantCultureIgnoreCase));
        }
        else
        {
            comboBox.SelectedItem = null;
        }
    }

    private void TbOnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            ApplyPadding(sender);
        }
    }

    private void TbOnSpinned(object? sender, SpinEventArgs e)
    {
        ApplyPadding(sender);
    }

    public event EventHandler? ValueApplied;

    #region Border

    private void CpBorder_OnColorChanged(object? sender, ColorChangedEventArgs e)
    {
        if (_ignoreEvents)
        {
            return;
        }

        if (DataContext is CssStyle style)
        {
            style.BorderColor = e.NewColor;
            ValueApplied?.Invoke(this, EventArgs.Empty);
        }
    }

    private void CbBorderAllStyle_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_ignoreEvents)
        {
            return;
        }

        if (DataContext is CssStyle style && CbBorderAllStyle.SelectedItem is ComboBoxItem item)
        {
            style.BorderStyle = item.Tag!.ToString();
            ValueApplied?.Invoke(this, EventArgs.Empty);
        }
    }

    private void ApplyBorderAllWidth()
    {
        if (DataContext is CssStyle style)
        {
            style.BorderWidth = (int?) TbBorderAllWidth.Value;
            ValueApplied?.Invoke(this, EventArgs.Empty);
        }
    }

    private void ApplyBorderAllCornerRadius()
    {
        if (DataContext is CssStyle style)
        {
            style.CornerRadius = (int?) TbBorderAllCornerRadius.Value;
            ValueApplied?.Invoke(this, EventArgs.Empty);
        }
    }

    private void TbBorderAllWidth_OnSpinned(object? sender, SpinEventArgs e)
    {
        ApplyBorderAllWidth();
    }

    private void TbBorderAllWidth_OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            ApplyBorderAllWidth();
        }
    }

    private void TbBorderAllCornerRadius_OnSpinned(object? sender, SpinEventArgs e)
    {
        ApplyBorderAllCornerRadius();
    }

    private void TbBorderAllCornerRadius_OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            ApplyBorderAllCornerRadius();
        }
    }

    #endregion
}