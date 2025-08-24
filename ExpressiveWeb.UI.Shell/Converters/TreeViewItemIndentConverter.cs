// *********************************************************
// 
// ExpressiveWeb.UI.Shell ss.cs
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

using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;

namespace ExpressiveWeb.UI.Shell.Converters;

public class TreeViewItemIndentConverter : IMultiValueConverter
{
    public static readonly TreeViewItemIndentConverter Instance = new();

    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count > 1 && values[0] is int level && values[1] is double indent)
        {
            return new Thickness(indent * level, 0, 0, 0);
        }

        return new Thickness(0);
    }
}