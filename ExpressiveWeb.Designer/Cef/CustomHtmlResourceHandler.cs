// *********************************************************
// 
// ExpressiveWeb.Designer CustomHtmlResourceHandler.cs
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

public class CustomHtmlResourceHandler : CefResourceHandler
{
    private byte[] _htmlBytes;
    private int _position;

    public CustomHtmlResourceHandler()
    {
        // Votre contenu HTML personnalisé ici
        string html = "<html><body><h1>Bonjour depuis CEFGlue !</h1></body></html>";
        _htmlBytes = Encoding.UTF8.GetBytes(html);
        _position = 0;
    }

    protected override bool Open(CefRequest request, out bool handleRequest, CefCallback callback)
    {
        handleRequest = true;
        return true;
    }

    protected override bool ProcessRequest(CefRequest request, CefCallback callback)
    {
        // Démarre la réponse immédiatement
        callback.Continue();
        return true;
    }

    protected override void GetResponseHeaders(CefResponse response, out long responseLength, out string redirectUrl)
    {
        response.MimeType = "text/html";
        response.Status = 200;
        response.StatusText = "OK";

        responseLength = _htmlBytes.Length;
        redirectUrl = null;
    }

    protected override bool Skip(long bytesToSkip, out long bytesSkipped, CefResourceSkipCallback callback)
    {
        throw new NotImplementedException();
    }

    protected override bool Read(Stream response, int bytesToRead, out int bytesRead, CefResourceReadCallback callback)
    {
        int toCopy = Math.Min(_htmlBytes.Length - _position, 4096);
        response.Write(_htmlBytes, _position, toCopy);
        _position += toCopy;
        bytesRead = toCopy;
        return _position <= _htmlBytes.Length;
    }

    protected override void Cancel()
    {
        
    }

   
}