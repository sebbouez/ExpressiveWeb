// *********************************************************
// 
// ExpressiveWeb.Designer PageRequestRouter.cs
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
using ExpressiveWeb.Core.FileManagement;
using ExpressiveWeb.Core.Html;
using ExpressiveWeb.Designer.Filters;

namespace ExpressiveWeb.Designer.Cef.Impl;

internal class PageRequestRouter : InternalResourceRouterBase
{
    private readonly HtmlFilterService _htmlFilterService = new();

    public PageRequestRouter()
    {
        _htmlFilterService.UseFilter<AddEditorReferencesFilter>();
    }

    internal override InternalRouterResult? Execute(string path)
    {
        InternalRouterResult result = new();

        if (File.Exists(path))
        {
            // filter the HTML before rendering
            string fileContent = FilesAccessHelper.ReadAllText(path);
            fileContent = _htmlFilterService.Filter(fileContent);

            result.Content = Encoding.UTF8.GetBytes(fileContent);
            result.MimeType = "text/html";
            result.Status = 200;

            return result;
        }

        return null;
    }
}