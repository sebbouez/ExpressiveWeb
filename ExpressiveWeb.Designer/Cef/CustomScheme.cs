// *********************************************************
// 
// ExpressiveWeb.Designer CustomScheme.cs
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

using System.Buffers.Text;
using System.Diagnostics;
using System.Text;
using ExpressiveWeb.Designer.Cef.Impl;
using Xilium.CefGlue;
using Xilium.CefGlue.Common.Handlers;

namespace ExpressiveWeb.Designer.Cef;

public class CustomSchemeHandler : CefSchemeHandlerFactory
{
    public const string LOCAL_FILE_SCHEME = "pfinternal";
    private const string HTMLLocalFileError = "<html><body><h1>Sorry an error occured.</h1></body></html>";

    protected override CefResourceHandler Create(CefBrowser browser, CefFrame frame, string schemeName, CefRequest request)
    {
        string url = request.Url;

        InternalRouterResult? result = null;

        Stopwatch sw = new();
        sw.Start();

        if (url.StartsWith(string.Concat(LOCAL_FILE_SCHEME, "://p/"), StringComparison.InvariantCultureIgnoreCase))
        {
            result = new PageRequestRouter().Execute(DecodeUrl(url));
        }

        if (url.StartsWith(string.Concat(LOCAL_FILE_SCHEME, "://f/"), StringComparison.InvariantCultureIgnoreCase))
        {
            result = new StaticResourcesRouter().Execute(DecodeUrl(url));
        }

        if (url.StartsWith(string.Concat(LOCAL_FILE_SCHEME, "://d/"), StringComparison.InvariantCultureIgnoreCase))
        {
            result = new DynamicResourcesRouter().Execute(DecodeUrl(url));
        }

        sw.Stop();

        if (result != null && result.Content != null)
        {
            DefaultResourceHandler response = new()
            {
                MimeType = result.MimeType,
                Status = result.Status,
                Response = new MemoryStream(result.Content)
            };

            response.Headers.Add("x-served-by", "internal");
            response.Headers.Add("x-duration", sw.ElapsedMilliseconds.ToString());

            return response;
        }

        return new DefaultResourceHandler
        {
            Response = new MemoryStream(Encoding.UTF8.GetBytes(HTMLLocalFileError)),
            Status = 404
        };
    }

    private string DecodeUrl(string url)
    {
        url = url.Replace($"{LOCAL_FILE_SCHEME}://", "", StringComparison.Ordinal);
        string base64Query = url.Split('/').Last();

        if (Base64.IsValid(base64Query))
        {
            return Encoding.ASCII.GetString(Convert.FromBase64String(base64Query));
        }

        return base64Query;
    }
}