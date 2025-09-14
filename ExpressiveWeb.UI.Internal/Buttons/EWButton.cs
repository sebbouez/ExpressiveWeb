// *********************************************************
// 
// ExpressiveWeb.UI.Shell EWButton.cs
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
using Avalonia.Interactivity;
using Avalonia.Media;

namespace ExpressiveWeb.Presentation.Buttons;

public class EWButton : Button
{
    public static readonly StyledProperty<string> TextProperty =
        AvaloniaProperty.Register<EWButton, string>(nameof(Text), string.Empty);

    public static readonly StyledProperty<IBrush?> IconBrushProperty =
        AvaloniaProperty.Register<EWButton, IBrush?>(nameof(IconBrush), null);

    public EWButton()
    {
        Loaded += OnLoaded;
    }

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        UpdatePseudoClasses();
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

    public string Text
    {
        get
        {
            return GetValue(TextProperty);
        }
        set
        {
            SetValue(TextProperty, value);
        }
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(":hasIcon", IconBrush != null);
    }
}