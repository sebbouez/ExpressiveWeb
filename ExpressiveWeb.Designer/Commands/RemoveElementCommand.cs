// *********************************************************
// 
// ExpressiveWeb.Designer RemoveElementCommand.cs
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

internal class RemoveElementCommand : IBusinessCommand
{
    private readonly HtmlEditor _editor;

    public RemoveElementCommand(HtmlEditor editor)
    {
        _editor = editor;
    }

    internal required HtmlElementInfo SourceElementInfo
    {
        get;
        set;
    }

    public void Do()
    {
        _editor.InternalCallBrowserMethod(string.Concat(HtmlEditor.JS_GLOBAL_EDITOR_OBJ_NAME, ".domHelper.removeElementJson"), SourceElementInfo);
        _editor.UpdateDecorators();
    }

    public void Undo()
    {
        _editor.InternalCallBrowserMethod(string.Concat(HtmlEditor.JS_GLOBAL_EDITOR_OBJ_NAME, ".domHelper.insertElementJson"), SourceElementInfo);
        _editor.UpdateDecorators();
        _editor.SelectElementByInternalId(SourceElementInfo.InternalId);
    }
}