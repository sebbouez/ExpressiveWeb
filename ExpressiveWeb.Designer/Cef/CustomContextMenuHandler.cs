// *********************************************************
// 
// ExpressiveWeb.Designer ContextMenuHandler.cs
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

using Xilium.CefGlue;
using Xilium.CefGlue.Common.Handlers;

namespace ExpressiveWeb.Designer.Cef;

public class CustomContextMenuHandler : ContextMenuHandler
{

    protected override bool RunContextMenu(CefBrowser browser, CefFrame frame, CefContextMenuParams parameters, CefMenuModel model, CefRunContextMenuCallback callback)
    {
        //ContextMenuOpening?.Invoke(this, EventArgs.Empty);
        return false;
    }

    protected override void OnBeforeContextMenu(CefBrowser browser, CefFrame frame, CefContextMenuParams state, CefMenuModel model)
    {
        model.Clear();
        base.OnBeforeContextMenu(browser, frame, state, model);
    }

    protected override void OnContextMenuDismissed(CefBrowser browser, CefFrame frame)
    {
        
    }
}