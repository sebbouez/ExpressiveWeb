// *********************************************************
// 
// ExpressiveWeb.Designer InternalJavascriptBridge.cs
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

using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using ExpressiveWeb.Core.Kit;
using ExpressiveWeb.Core.Kit.ComponentFeatures;
using ExpressiveWeb.Core.Log;
using ExpressiveWeb.Designer.Commands;
using ExpressiveWeb.Designer.Filters;
using ExpressiveWeb.Designer.Models;

namespace ExpressiveWeb.Designer;

[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Used by Javascript, no reference in C# code")]
[SuppressMessage("Trimming", "IL2026:Members annotated with \'RequiresUnreferencedCodeAttribute\' require dynamic access otherwise can break functionality when trimming application code")]
[SuppressMessage("AOT", "IL3050:Calling members annotated with \'RequiresDynamicCodeAttribute\' may break functionality when AOT compiling.")]
public class InternalJavascriptBridge
{
    private readonly HtmlEditor _editor;
    private readonly ILogService _logService;

    public InternalJavascriptBridge(HtmlEditor editor, ILogService logService)
    {
        _editor = editor;
        _logService = logService;
    }

    public void ComponentActionMenuOpening(double x, double y)
    {
        _editor.InvokeComponentActionMenuOpening(x, y);
    }

    public void ContextMenuOpening(double x, double y)
    {
        _editor.InvokeContextMenuOpening(x, y);
    }

    public void DropElement(string sourceElementInfoJson, string targetElementInfoJson, int relativePosition, bool ctrlPressed)
    {
        HtmlElementInfo? sourceElementInfo = JsonSerializer.Deserialize<HtmlElementInfo?>(sourceElementInfoJson);
        HtmlElementInfo? targetElementInfo = JsonSerializer.Deserialize<HtmlElementInfo?>(targetElementInfoJson);

        if (sourceElementInfo == null || targetElementInfo == null)
        {
            _logService.Error("DropElement: source or target element info is null");
            return;
        }

        if (ctrlPressed)
        {
            HtmlElementInfo elementCopy = sourceElementInfo.Freeze();
            elementCopy.InternalId = Guid.NewGuid().ToString();
            elementCopy.ParentInternalId = targetElementInfo.ParentInternalId;
            elementCopy.Index = HtmlEditor.GetElementIndex(sourceElementInfo, targetElementInfo, (MoveRelativePosition)relativePosition);
            elementCopy.InnerHtml = _editor.HTMLFilterService.FilterWith<RemoveEditorInternalIdFilter>(elementCopy.InnerHtml);

            InsertElementCommand cmd = new(_editor)
            {
                SourceElementInfo = elementCopy
            };
            _editor.CommandManager.ExecuteCommand(cmd);
        }
        else
        {
            MoveElementCommand cmd = new(_editor)
            {
                SourceElementInfo = sourceElementInfo,
                TargetElementInfo = targetElementInfo,
                RelativePosition = (MoveRelativePosition) relativePosition
            };
            _editor.CommandManager.ExecuteCommand(cmd);
        }
    }

    public void RaiseElementClick(string json)
    {
        HtmlElementInfo? elementInfo = JsonSerializer.Deserialize<HtmlElementInfo?>(json);

        if (_editor.IsTextEditing)
        {
            _ = _editor.EndTextEditing();
            return;
        }

        _editor.InvokeSelectionChanged(elementInfo);
    }

    public void RaiseElementDblClick(string json)
    {
        HtmlElementInfo? elementInfo = JsonSerializer.Deserialize<HtmlElementInfo?>(json);

        if (elementInfo == null)
        {
            _logService.Error("RaiseElementDblClick: element info is null");
            return;
        }

        KitComponent? component = HtmlEditor.Kit.Components.FirstOrDefault(x => x.UID.Equals(elementInfo.ComponentUid));
        if (component != null && component.HasFeature<InlineEditFeature>() && component.Allows("text"))
        {
            _editor.StartTextEditing(elementInfo);
        }
    }

    public void RaiseScroll()
    {
        _editor.InvokeComponentActionMenuClose();
    }

    public void RaiseSelectedElementChanged(string json)
    {
        HtmlElementInfo? elementInfo = JsonSerializer.Deserialize<HtmlElementInfo>(json);
        _editor.InvokeSelectionChanged(elementInfo);
    }

    public void RaiseTextSelected(string json)
    {
        TextSelectionInfo? textSelectionInfo = JsonSerializer.Deserialize<TextSelectionInfo>(json);
        _editor.InvokeSelectionChanged(textSelectionInfo);
    }
}