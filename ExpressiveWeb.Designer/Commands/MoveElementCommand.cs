// *********************************************************
// 
// ExpressiveWeb.Designer MoveElementCommand.cs
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

using System.Globalization;
using ExpressiveWeb.Core.Commands;
using ExpressiveWeb.Designer.Models;

namespace ExpressiveWeb.Designer.Commands;

/// <summary>
/// Command that will move an HTML to another place in the same document
/// </summary>
internal class MoveElementCommand : IBusinessCommand
{
    private readonly HtmlEditor _editor;

    private int _oldIndex;
    private string _oldParentInternalId;

    internal MoveElementCommand(HtmlEditor editor)
    {
        _editor = editor;
    }

    internal HtmlElementInfo SourceElementInfo
    {
        get;
        set;
    }

    internal HtmlElementInfo TargetElementInfo
    {
        get;
        set;
    }

    internal int RelativePosition
    {
        get;
        set;
    }

    public void Do()
    {
        _oldIndex = SourceElementInfo.Index;
        _oldParentInternalId = SourceElementInfo.ParentInternalId;

        string script;
        switch (RelativePosition)
        {
            case -1:

                int newIndex = TargetElementInfo.Index;
                if (SourceElementInfo.ParentInternalId == TargetElementInfo.ParentInternalId
                    && SourceElementInfo.Index < TargetElementInfo.Index)
                {
                    newIndex--;
                }

                script = string.Format(CultureInfo.InvariantCulture, ".domHelper.moveElement('{0}','{1}', {2})", SourceElementInfo.InternalId, TargetElementInfo.ParentInternalId, newIndex);
                break;
            case 1:

                int idx = TargetElementInfo.Index + 1;
                if (SourceElementInfo.ParentInternalId == TargetElementInfo.ParentInternalId
                    && SourceElementInfo.Index < TargetElementInfo.Index)
                {
                    idx--;
                }

                script = string.Format(CultureInfo.InvariantCulture, ".domHelper.moveElement('{0}','{1}', {2})", SourceElementInfo.InternalId, TargetElementInfo.ParentInternalId, idx);
                break;
            case 0:
                script = string.Format(CultureInfo.InvariantCulture, ".domHelper.moveElement('{0}','{1}', {2})", SourceElementInfo.InternalId, TargetElementInfo.InternalId, 99);
                break;
            default:
                script = string.Empty;
                break;
        }

        _editor.InternalExecuteBrowserScript(string.Concat(HtmlEditor.JS_GLOBAL_EDITOR_OBJ_NAME, script));
        _editor.UpdateDecorators();
        _editor.SelectElementByInternalId(SourceElementInfo.InternalId);
    }

    public void Undo()
    {
        string script = string.Format(CultureInfo.InvariantCulture, ".domHelper.moveElement('{0}','{1}', {2})", SourceElementInfo.InternalId, _oldParentInternalId, _oldIndex);
        _editor.InternalExecuteBrowserScript(string.Concat(HtmlEditor.JS_GLOBAL_EDITOR_OBJ_NAME, script));
        _editor.UpdateDecorators();
        _editor.SelectElementByInternalId(SourceElementInfo.InternalId);
    }
}