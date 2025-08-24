// *********************************************************
// 
// ExpressiveWeb.Designer HtmlElementInfo.cs
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

internal sealed class HtmlElementInfo
{
    [JsonPropertyName("componentUid")]
    public string ComponentUid
    {
        get;
        set;
    }

    [JsonPropertyName("internalId")]
    public string InternalId
    {
        get;
        set;
    }

    [JsonPropertyName("innerHtml")]
    public string InnerHtml
    {
        get;
        set;
    }

    [JsonPropertyName("innerText")]
    public string InnerText
    {
        get;
        set;
    }

    [JsonPropertyName("tagName")]
    public string TagName
    {
        get;
        set;
    }

    [JsonPropertyName("cssClass")]
    public string CssClass
    {
        get;
        set;
    }

    [JsonPropertyName("parentInternalId")]
    public string ParentInternalId
    {
        get;
        set;
    }

    [JsonPropertyName("index")]
    public int Index
    {
        get;
        set;
    }

    [JsonPropertyName("attributes")]
    public List<HTmlElementAttributeInfo> Attributes
    {
        get;
        set;
    } = new();

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }

    [RequiresUnreferencedCode("Calls System.Text.Json.JsonSerializer.Serialize<TValue>(TValue, JsonSerializerOptions)")]
    [RequiresDynamicCode("Calls System.Text.Json.JsonSerializer.Serialize<TValue>(TValue, JsonSerializerOptions)")]
    internal HtmlElementInfo Freeze()
    {
        string json = JsonSerializer.Serialize(this);
        return JsonSerializer.Deserialize<HtmlElementInfo>(json)!;
    }
}