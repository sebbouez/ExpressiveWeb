// *********************************************************
// 
// ExpressiveWeb.Designer TextSelectionInfo.cs
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
using System.Text.Json.Serialization;

namespace ExpressiveWeb.Designer.Models;

[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
internal sealed class TextSelectionInfo
{
    [JsonPropertyName("canCreateLink")]
    public bool CanCreateLink
    {
        get;
        set;
    }

    [JsonPropertyName("canUnLink")]
    public bool CanUnLink
    {
        get;
        set;
    }

    [JsonPropertyName("isSelectionStrikeThrough")]
    public bool IsSelectionStrikeThrough
    {
        get;
        set;
    }

    [JsonPropertyName("isSelectionSubScript")]
    public bool IsSelectionSubScript
    {
        get;
        set;
    }

    [JsonPropertyName("isSelectionSuperScript")]
    public bool IsSelectionSuperScript
    {
        get;
        set;
    }

    [JsonPropertyName("isSelectionBold")]
    public bool IsSelectionBold
    {
        get;
        set;
    }

    [JsonPropertyName("isSelectionBulletList")]
    public bool IsSelectionBulletList
    {
        get;
        set;
    }

    [JsonPropertyName("isSelectionCenterAlign")]
    public bool IsSelectionCenterAlign
    {
        get;
        set;
    }

    [JsonPropertyName("isSelectionItalic")]
    public bool IsSelectionItalic
    {
        get;
        set;
    }

    [JsonPropertyName("isSelectionJustifyAlign")]
    public bool IsSelectionJustifyAlign
    {
        get;
        set;
    }

    [JsonPropertyName("isSelectionLeftAlign")]
    public bool IsSelectionLeftAlign
    {
        get;
        set;
    }

    [JsonPropertyName("isSelectionNumberList")]
    public bool IsSelectionNumberList
    {
        get;
        set;
    }

    [JsonPropertyName("isSelectionRightAlign")]
    public bool IsSelectionRightAlign
    {
        get;
        set;
    }

    [JsonPropertyName("isSelectionUnderline")]
    public bool IsSelectionUnderline
    {
        get;
        set;
    }

    [JsonPropertyName("parentTagName")]
    public string ParentTagName
    {
        get;
        set;
    } = string.Empty;

    [JsonPropertyName("selectedText")]
    public string SelectedText
    {
        get;
        set;
    } = string.Empty;

    [JsonPropertyName("selectionClassName")]
    public string SelectionClassName
    {
        get;
        set;
    } = string.Empty;

    [JsonPropertyName("selectionRangeRectHeight")]
    public double SelectionRangeRectHeight
    {
        get;
        set;
    } 

    [JsonPropertyName("selectionRangeRectWidth")]
    public double SelectionRangeRectWidth
    {
        get;
        set;
    }

    [JsonPropertyName("selectionRangeRectX")]
    public double SelectionRangeRectX
    {
        get;
        set;
    }

    [JsonPropertyName("selectionRangeRectY")]
    public double SelectionRangeRectY
    {
        get;
        set;
    }

    [JsonPropertyName("selectionTagName")]
    public string SelectionTagName
    {
        get;
        set;
    } = string.Empty;

    [RequiresUnreferencedCode("Calls System.Text.Json.JsonSerializer.Serialize<TValue>(TValue, JsonSerializerOptions)")]
    [RequiresDynamicCode("Calls System.Text.Json.JsonSerializer.Serialize<TValue>(TValue, JsonSerializerOptions)")]
    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}