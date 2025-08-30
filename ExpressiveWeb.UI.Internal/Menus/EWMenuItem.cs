// *********************************************************
// 
// ExpressiveWeb.UI.Shell EWMenuItem.cs
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
using Avalonia.Media;
using Avalonia.Threading;

namespace ExpressiveWeb.Presentation.Menus;

public class EWMenuItem : MenuItem
{
    public static readonly StyledProperty<IBrush?> IconBrushProperty =
        AvaloniaProperty.Register<EWMenuItem, IBrush?>(nameof(IconBrush), Brushes.Transparent);

    public EWMenuItem()
    {
        PropertyChanged += OnPropertyChanged;
    }

    private void OnPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property.Name == nameof(IsEnabled))
        {
            Dispatcher.UIThread.Post(() =>
            {
                this.UpdateIsEffectivelyEnabled();
                UpdateLayout();
                InvalidateVisual();
                PseudoClasses.Set(":disabled", !IsEnabled);
            });
        }
    }

    public IBrush? IconBrush
    {
        get
        {
            return GetValue(IconBrushProperty);
        }
        set
        {
            SetValue(IconBrushProperty, value);
        }
    }
}