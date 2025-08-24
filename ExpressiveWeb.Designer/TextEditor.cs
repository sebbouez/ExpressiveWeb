// *********************************************************
// 
// ExpressiveWeb.Designer TextEditor.cs
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

using ExpressiveWeb.Designer.Commands;
using ExpressiveWeb.Designer.Models;

namespace ExpressiveWeb.Designer;

internal class TextEditor
{
    private readonly HtmlEditor _editor;
    private readonly HtmlElementInfo _elementInfo;

    public TextEditor(HtmlEditor editor, HtmlElementInfo elementInfo)
    {
        _editor = editor;
        _elementInfo = elementInfo;
    }

    public void ToggleBold()
    {
    }

    public void ToggleItalic()
    {
    }

    public void ToggleUnderline()
    {
    }

    internal async Task Commit()
    {
        HtmlElementInfo? info = await _editor.GetElementInfoFromInternalId(_elementInfo.InternalId);

        if (info != null && info.InnerText.Trim() != _elementInfo.InnerText.Trim())
        {
            EditElementInnerHtmlCommand cmd = new(_editor)
            {
                InitialElementInfo = _elementInfo,
                FinalElementInfo = info
            };
            _editor.CommandManager.ExecuteCommandSilent(cmd);
        }

        _editor.InternalCallBrowserMethod(string.Concat(HtmlEditor.JS_GLOBAL_EDITOR_OBJ_NAME, ".adornerManager.exitTextEditMode"));
        _editor.SelectElementByInternalId(_elementInfo.InternalId);
    }

    internal void Start()
    {
        _editor.InternalCallBrowserMethod(string.Concat(HtmlEditor.JS_GLOBAL_EDITOR_OBJ_NAME, ".adornerManager.startTextEditMode"), _elementInfo.InternalId);
    }
}