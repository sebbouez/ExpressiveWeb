// *********************************************************
// 
// ExpressiveWeb.UI.Internal EWToolbarButton.cs
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

namespace ExpressiveWeb.Presentation.Menus;

public class EWToolbarButton : Button
{
    public static readonly StyledProperty<IBrush?> IconBrushProperty =
        AvaloniaProperty.Register<EWToolbarButton, IBrush?>(nameof(IconBrush));

    public static readonly StyledProperty<bool> IsInOverflowProperty =
        AvaloniaProperty.Register<EWToolbarButton, bool>(nameof(IsInOverflow));

    public static readonly StyledProperty<string> TextProperty =
        AvaloniaProperty.Register<EWToolbarButton, string>(nameof(Text), string.Empty);

    public EWToolbarButton()
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

    public bool IsInOverflow
    {
        get
        {
            return GetValue(IsInOverflowProperty);
        }
        set
        {
            SetValue(IsInOverflowProperty, value);
            UpdatePseudoClasses();
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
        PseudoClasses.Set(":isOverFlown", IsInOverflow);
    }
}