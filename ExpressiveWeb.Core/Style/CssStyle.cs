// *********************************************************
// 
// ExpressiveWeb.Core CssStyle.cs
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

using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using Color = System.Drawing.Color;

namespace ExpressiveWeb.Core.Style;

public class CssStyle : INotifyPropertyChanged
{
    private Avalonia.Media.Color? _backgroundColor;
    private Avalonia.Media.Color? _borderColor;
    private int? _paddingBottom;
    private int? _paddingLeft;
    private int? _paddingRight;
    private int? _paddingTop;
    private int? _marginBottom;
    private int? _marginLeft;
    private int? _marginRight;
    private int? _marginTop;
    private int? _borderWidth;
    private string? _borderStyle;
    private int? _cornerRadius;

    [Category("Colors")]
    [AutoMapStyle("background-color")]
    public Avalonia.Media.Color? BackgroundColor
    {
        get
        {
            return _backgroundColor;
        }
        set
        {
            SetField(ref _backgroundColor, value);
        }
    }

    [Category("Colors")]
    [AutoMapStyle("border-color")]
    public Avalonia.Media.Color? BorderColor
    {
        get
        {
            return _borderColor;
        }
        set
        {
            SetField(ref _borderColor, value);
        }
    }

    [Category("Border")]
    [AutoMapStyle("border-width")]
    public int? BorderWidth
    {
        get
        {
            return _borderWidth;
        }
        set
        {
            SetField(ref _borderWidth, value);
        }
    }

    [Category("Border")]
    [AutoMapStyle("border-style")]
    public string? BorderStyle
    {
        get
        {
            return _borderStyle;
        }
        set
        {
            SetField(ref _borderStyle, value);
        }
    }

    [Category("Border")]
    [AutoMapStyle("border-radius")]
    public int? CornerRadius
    {
        get
        {
            return _cornerRadius;
        }
        set
        {
            SetField(ref _cornerRadius, value);
        }
    }

    [Category("Padding")]
    [AutoMapStyle("padding-bottom")]
    public int? PaddingBottom
    {
        get
        {
            return _paddingBottom;
        }
        set
        {
            SetField(ref _paddingBottom, value);
        }
    }

    [Category("Padding")]
    [AutoMapStyle("padding-left")]
    public int? PaddingLeft
    {
        get
        {
            return _paddingLeft;
        }
        set
        {
            SetField(ref _paddingLeft, value);
        }
    }

    [Category("Padding")]
    [AutoMapStyle("padding-right")]
    public int? PaddingRight
    {
        get
        {
            return _paddingRight;
        }
        set
        {
            SetField(ref _paddingRight, value);
        }
    }

    [Category("Padding")]
    [AutoMapStyle("padding-top")]
    public int? PaddingTop
    {
        get
        {
            return _paddingTop;
        }
        set
        {
            SetField(ref _paddingTop, value);
        }
    }

    [Category("Margin")]
    [AutoMapStyle("margin-left")]
    public int? MarginLeft
    {
        get
        {
            return _marginLeft;
        }
        set
        {
            SetField(ref _marginLeft, value);
        }
    }

    [Category("Margin")]
    [AutoMapStyle("margin-top")]
    public int? MarginTop
    {
        get
        {
            return _marginTop;
        }
        set
        {
            SetField(ref _marginTop, value);
        }
    }

    [Category("Margin")]
    [AutoMapStyle("margin-bottom")]
    public int? MarginBottom
    {
        get
        {
            return _marginBottom;
        }
        set
        {
            SetField(ref _marginBottom, value);
        }
    }

    [Category("Margin")]
    [AutoMapStyle("margin-right")]
    public int? MarginRight
    {
        get
        {
            return _marginRight;
        }
        set
        {
            SetField(ref _marginRight, value);
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    private string MediaColorToHexColor(Avalonia.Media.Color? color)
    {
        if (color == null)
        {
            return string.Empty;
        }

        return $"#{color.Value.R:X2}{color.Value.G:X2}{color.Value.B:X2}";
    }

    public override string ToString()
    {
        StringBuilder str = new();

        if (BackgroundColor != null)
        {
            str.Append($"background-color:{MediaColorToHexColor(_backgroundColor)};");
        }

        if (_borderWidth.HasValue && _borderWidth.Value > 0)
        {
            str.Append($"border:{_borderWidth}px {_borderStyle} {MediaColorToHexColor(_borderColor)};");
        }


        if (_paddingLeft.HasValue)
        {
            str.Append($"padding-left:{_paddingLeft}px;");
        }

        if (_paddingRight.HasValue)
        {
            str.Append($"padding-right:{_paddingRight}px;");
        }

        if (_paddingTop.HasValue)
        {
            str.Append($"padding-top:{_paddingTop}px;");
        }

        if (_paddingBottom.HasValue)
        {
            str.Append($"padding-bottom:{_paddingBottom}px;");
        }

        if (_marginLeft.HasValue)
        {
            str.Append($"margin-left:{_marginLeft}px;");
        }

        if (_marginRight.HasValue)
        {
            str.Append($"margin-right:{_marginRight}px;");
        }

        if (_marginTop.HasValue)
        {
            str.Append($"margin-top:{_marginTop}px;");
        }

        if (_marginBottom.HasValue)
        {
            str.Append($"margin-bottom:{_marginBottom}px;");
        }

        return str.ToString();
    }
}