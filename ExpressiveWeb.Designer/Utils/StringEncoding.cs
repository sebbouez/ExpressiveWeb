// *********************************************************
// 
// ExpressiveWeb.Designer StringEncoding.cs
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
using System.Text;
using ExpressiveWeb.Designer.Extensions;

namespace ExpressiveWeb.Designer.Utils;

internal class StringEncoding
{
    // Extensions methods taken from CefSharp
    // They facilitate the use of Js methods with arguments
    // https://github.com/cefsharp/CefSharp/blob/master/CefSharp/WebBrowserExtensions.cs

    private static Func<string, string> EncodeScriptParam
    {
        get;
        set;
    } = str =>
    {
        return str.Replace("\\", "\\\\")
            .Replace("'", "\\'")
            .Replace("\t", "\\t")
            .Replace("\r", "\\r")
            .Replace("\n", "\\n");
    };

    internal static string GetScriptForJavascriptMethodWithArgs(string methodName, object[] args)
    {
        StringBuilder stringBuilder = new();
        stringBuilder.Append(methodName);
        stringBuilder.Append("(");

        if (args.Length > 0)
        {
            for (int i = 0; i < args.Length; i++)
            {
                object? obj = args[i];
                if (obj == null)
                {
                    stringBuilder.Append("null");
                }
                else if (obj.IsNumeric())
                {
                    stringBuilder.Append(Convert.ToString(args[i], CultureInfo.InvariantCulture));
                }
                else if (obj is bool)
                {
                    stringBuilder.Append(args[i].ToString().ToLowerInvariant());
                }
                else
                {
                    stringBuilder.Append("'");
                    stringBuilder.Append(EncodeScriptParam(obj.ToString()));
                    stringBuilder.Append("'");
                }

                stringBuilder.Append(", ");
            }

            //Remove the trailing comma
            stringBuilder.Remove(stringBuilder.Length - 2, 2);
        }

        stringBuilder.Append(");");

        return stringBuilder.ToString();
    }
}