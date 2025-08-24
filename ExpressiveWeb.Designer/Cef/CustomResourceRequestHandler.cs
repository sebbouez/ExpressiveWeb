// *********************************************************
// 
// ExpressiveWeb.Designer CustomResourceRequestHandler.cs
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

using ExpressiveWeb.Core;
using ExpressiveWeb.Core.Log;
using ExpressiveWeb.Core.Network;
using Microsoft.Extensions.DependencyInjection;
using Xilium.CefGlue;

namespace ExpressiveWeb.Designer.Cef;

public class CustomResourceRequestHandler : CefResourceRequestHandler
{
    private readonly ILogService? _logService;
    private readonly INetworkService? _networkService;

    public CustomResourceRequestHandler()
    {
        _networkService = AppServices.ServicesFactory?.GetService<INetworkService>();
        _logService = AppServices.ServicesFactory?.GetService<ILogService>();
    }

    protected override CefCookieAccessFilter? GetCookieAccessFilter(CefBrowser browser, CefFrame frame, CefRequest request)
    {
        return null;
    }

    protected override CefResourceHandler? GetResourceHandler(CefBrowser browser, CefFrame frame, CefRequest request)
    {
        if (request.Url.StartsWith($"{CustomSchemeHandler.LOCAL_FILE_SCHEME}://", StringComparison.InvariantCultureIgnoreCase))
        {
            return null;
        }

        if (!_networkService!.IsConnected() && request.Url.StartsWith("http", StringComparison.InvariantCultureIgnoreCase))
        {
            _logService!.Info($"Request to {request.Url} is canceled because we are offline.");
            return new OfflineResourceHandler();
        }

        if (_networkService.AcceptOnlyHttps && request.Url.StartsWith("http://", StringComparison.InvariantCultureIgnoreCase))
        {
            _logService!.Info($"Request to {request.Url} is canceled because we only allow https resources.");
            return new OfflineResourceHandler();
        }

        return null;
    }
}