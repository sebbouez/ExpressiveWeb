// *********************************************************
// 
// ExpressiveWeb.Core StringExtensions.cs
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

namespace ExpressiveWeb.Core.Extensions;

public static class StringExtensions
{
    /// <summary>
    /// Determines whether a string matches any of the values provided in the specified delimited string.
    /// </summary>
    /// <param name="str">The string to compare.</param>
    /// <param name="values">A delimited string containing potential matches, with items separated by ',' or ';'.</param>
    /// <returns>True if the string matches any of the values; otherwise, false.</returns>
    public static bool In(this string str, string values)
    {
        if (string.IsNullOrEmpty(values))
        {
            return false;
        }

        string[] op = values.Split(new[] {';', ','}, StringSplitOptions.RemoveEmptyEntries);
        return str.In(op);
    }

    /// <summary>
    /// Determines whether a string matches any of the values provided in the specified array of strings.
    /// </summary>
    /// <param name="str">The string to compare.</param>
    /// <param name="values">An array of strings containing potential matches.</param>
    /// <returns>True if the string matches any of the values; otherwise, false.</returns>
    public static bool In(this string str, string[] values)
    {
        if (values.Length == 0)
        {
            return false;
        }

        foreach (string item in values)
        {
            if (!string.IsNullOrEmpty(item) && str.Equals(item.Trim(), StringComparison.InvariantCultureIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }
}