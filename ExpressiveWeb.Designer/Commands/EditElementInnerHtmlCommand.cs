// *********************************************************
// 
// ExpressiveWeb.Designer EditElementInnerHtmlCommand.cs
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

using ExpressiveWeb.Core.Commands;
using ExpressiveWeb.Designer.Models;

namespace ExpressiveWeb.Designer.Commands;

internal class EditElementInnerHtmlCommand : IBusinessCommand
{
    private readonly HtmlEditor _editor;

    internal EditElementInnerHtmlCommand(HtmlEditor editor)
    {
        _editor = editor;
    }

    internal required HtmlElementInfo InitialElementInfo
    {
        get;
        init;
    }

    internal required HtmlElementInfo FinalElementInfo
    {
        get;
        init;
    }

    public void Do()
    {
        _editor.InternalCallBrowserMethod(string.Concat(HtmlEditor.JS_GLOBAL_EDITOR_OBJ_NAME, ".domHelper.setElementInnerHtml"), InitialElementInfo.InternalId!, FinalElementInfo.InnerHtml??string.Empty);
        _editor.UpdateDecorators();
    }

    public void Undo()
    {
        _editor.InternalCallBrowserMethod(string.Concat(HtmlEditor.JS_GLOBAL_EDITOR_OBJ_NAME, ".domHelper.setElementInnerHtml"), InitialElementInfo.InternalId!, InitialElementInfo.InnerHtml??string.Empty);
        _editor.UpdateDecorators();
    }
}