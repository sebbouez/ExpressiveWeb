// *********************************************************
// 
// ExpressiveWeb.Core Styleervice.cs
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

using System.Reflection;
using ExCSS;
using ExpressiveWeb.Core.BackgroundServices;
using ExpressiveWeb.Core.Log;

namespace ExpressiveWeb.Core.Style;

public class StyleService : IStyleService
{
    private readonly ILogService _logService;
    private readonly IBackgroundTaskManager _taskManager;

    public StyleService(IBackgroundTaskManager taskManager, ILogService logService)
    {
        _logService = logService;
        _taskManager = taskManager;
    }

    public CssStyle ParseStyleAttribute(string style)
    {
        StylesheetParser parser = new();

        Stylesheet? stylesheet = parser.Parse(string.Concat("local{", style, "}"));

        IStyleRule? uniqueStyleRule = stylesheet.StyleRules.FirstOrDefault();
        if (uniqueStyleRule == null)
        {
            return new CssStyle();
        }

        CssStyle result = new();

        IEnumerable<PropertyInfo> mappingProperties = typeof(CssStyle).GetProperties().Where(p => p.GetCustomAttribute<AutoMapStyleAttribute>() != null);

        foreach (PropertyInfo mappingProperty in mappingProperties)
        {
            AutoMapStyleAttribute? attr = mappingProperty.GetCustomAttribute<AutoMapStyleAttribute>();
            if (attr == null)
            {
                continue;
            }

            string? value = uniqueStyleRule.Style.FirstOrDefault(x => x.Name.Equals(attr.AttributeName))?.Value;

            if (!string.IsNullOrEmpty(value) && mappingProperty.PropertyType == typeof(int?))
            {
                mappingProperty.SetValue(result, ParseInt(value));
            }
            else if (!string.IsNullOrEmpty(value) && mappingProperty.PropertyType == typeof(Avalonia.Media.Color?))
            {
                mappingProperty.SetValue(result, ParseColor(value));
            }
        }

        return result;
    }

    public async Task<Stylesheet> ParseStyleSheet(string styleSheetFilePath)
    {
        using StreamReader streamReader = new(styleSheetFilePath);


        StylesheetParser p = new();
        Stylesheet? styleSheet = await p.ParseAsync(streamReader.BaseStream);


        // foreach (IStyleRule styleRule in styleSheet.StyleRules)
        // {
        //     styleRule.Style
        // }

        // UserStyle s = new();
        // s.RuleSet.Add(new StyleProperty(PropertyNames.FontSize)
        // {
        //     Value = "14px"
        // });

        return styleSheet;
    }

    public string BuildStyleAttribute(CssStyle style)
    {
        StylesheetParser parser = new();


        return "";
    }

    private int? ParseInt(string value)
    {
        string numberPart = new(value.Where(char.IsDigit).ToArray());
        return int.TryParse(numberPart, out int tempVal) ? tempVal : null;
    }

    private Avalonia.Media.Color? ParseColor(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return null;
        }

        value = value.Trim().ToLowerInvariant();

        // Handle hex colors
        if (value.StartsWith("#", StringComparison.InvariantCulture))
        {
            if (int.TryParse(value[1..], System.Globalization.NumberStyles.HexNumber, null, out int colorValue))
            {
                byte r = (byte) ((colorValue >> 16) & 0xFF);
                byte g = (byte) ((colorValue >> 8) & 0xFF);
                byte b = (byte) (colorValue & 0xFF);
                return new Avalonia.Media.Color(255, r, g, b);
            }

            return null;
        }

        // Handle rgb/rgba function
        if (value.StartsWith("rgb", StringComparison.InvariantCulture))
        {
            string[] parts = value.Substring(value.IndexOf('(') + 1, value.IndexOf(')') - value.IndexOf('(') - 1)
                .Split(',', StringSplitOptions.TrimEntries);

            if (parts.Length >= 3 &&
                byte.TryParse(parts[0], out byte r) &&
                byte.TryParse(parts[1], out byte g) &&
                byte.TryParse(parts[2], out byte b))
            {
                byte a = 255;
                if (parts.Length > 3 && float.TryParse(parts[3], out float alpha))
                {
                    a = (byte) (alpha * 255);
                }

                return new Avalonia.Media.Color(a, r, g, b);
            }

            return null;
        }

        // Handle named colors
        return value switch
        {
            "transparent" => Avalonia.Media.Colors.Transparent,
            "red" => Avalonia.Media.Colors.Red,
            "green" => Avalonia.Media.Colors.Green,
            "blue" => Avalonia.Media.Colors.Blue,
            "white" => Avalonia.Media.Colors.White,
            "black" => Avalonia.Media.Colors.Black,
            _ => null
        };
    }
}

public interface IStyleService
{
    CssStyle ParseStyleAttribute(string style);
    Task<Stylesheet> ParseStyleSheet(string styleSheetFilePath);
}