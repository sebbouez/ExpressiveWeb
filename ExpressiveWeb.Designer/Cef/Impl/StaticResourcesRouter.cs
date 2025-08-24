// *********************************************************
// 
// ExpressiveWeb.Designer StaticResourcesRouter.cs
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

using System.Text;

namespace ExpressiveWeb.Designer.Cef.Impl;

internal class StaticResourcesRouter : InternalResourceRouterBase
{
    internal override InternalRouterResult Execute(string path)
    {
        InternalRouterResult result = new();

        if (path.EndsWith(".js", StringComparison.OrdinalIgnoreCase))
        {
            string js = ResourcesHelper.ResolveResource(path.Split('/').Last());
            result.MimeType = "text/javascript";
            result.Content = Encoding.UTF8.GetBytes(js);
        }

        if (path.EndsWith(".css", StringComparison.OrdinalIgnoreCase))
        {
            string css = ResourcesHelper.ResolveResource(path.Split('/').Last());
            result.MimeType = "text/css";
            result.Content = Encoding.UTF8.GetBytes(css);
        }

        return result;
    }
}