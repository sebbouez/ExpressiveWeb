// *********************************************************
// 
// ExpressiveWeb.Designer HtmlElement.cs
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

using ExpressiveWeb.Core.Kit;

namespace ExpressiveWeb.Designer.Models;

public sealed class HtmlElement
{
    public string InternalId
    {
        get;
        set;
    } = string.Empty;

    public KitComponent? KitComponent
    {
        get;
        set;
    }

    public string TagName
    {
        get;
        set;
    } = string.Empty;

    public string CssClass
    {
        get;
        set;
    } = string.Empty;

    public int Index
    {
        get;
        set;
    }

    public int ParentChildrenCount
    {
        get;
        internal set;
    }

    public string? GetAttribute(string name)
    {
        return Attributes.FirstOrDefault(a => a.Name == name)?.Value;
    }

    public List<HtmlElementAttribute> Attributes
    {
        get;
        set;
    } = new();

    internal HtmlElementInfo DataContext
    {
        get;
        init;
    } = null!;

    public bool HasCssClass(string cssClass)
    {
        return CssClass.Trim().Split(' ').Contains(cssClass, StringComparer.Ordinal);
    }
}