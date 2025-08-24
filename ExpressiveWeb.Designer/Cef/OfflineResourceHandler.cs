// *********************************************************
// 
// ExpressiveWeb.Designer OfflineResourceHandler.cs
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
using Xilium.CefGlue;

namespace ExpressiveWeb.Designer.Cef;

public class OfflineResourceHandler : CefResourceHandler
{
    private readonly byte[] _htmlBytes;
    private int _position;

    public OfflineResourceHandler()
    {
        _htmlBytes = Encoding.UTF8.GetBytes("");
        _position = 0;
    }

    protected override void Cancel()
    {
    }

    protected override void GetResponseHeaders(CefResponse response, out long responseLength, out string redirectUrl)
    {
        response.MimeType = "text/html";
        response.Status = 404;
        response.StatusText = "Not Found";

        response.SetHeaderByName("Access-Control-Allow-Origin", "*", true);

        responseLength = _htmlBytes.Length;
        redirectUrl = null;
    }

    protected override bool Open(CefRequest request, out bool handleRequest, CefCallback callback)
    {
        handleRequest = true;
        return true;
    }

    protected override bool ProcessRequest(CefRequest request, CefCallback callback)
    {
        callback.Continue();
        return true;
    }

    protected override bool Read(Stream response, int bytesToRead, out int bytesRead, CefResourceReadCallback callback)
    {
        int toCopy = Math.Min(_htmlBytes.Length - _position, 4096);
        response.Write(_htmlBytes, _position, toCopy);
        _position += toCopy;
        bytesRead = toCopy;
        return _position <= _htmlBytes.Length;
    }

    protected override bool Skip(long bytesToSkip, out long bytesSkipped, CefResourceSkipCallback callback)
    {
        bytesSkipped = _htmlBytes.Length;
        return true;
    }
}