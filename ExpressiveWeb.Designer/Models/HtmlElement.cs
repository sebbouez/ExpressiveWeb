using System.Text.Json.Serialization;
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

    public bool HasCssClass(string cssClass)
    {
        return CssClass.Trim().Split(' ').Contains(cssClass, StringComparer.Ordinal);
    }

    public List<HtmlElementAttribute> Attributes
    {
        get;
        set;
    } = new();
    
    internal HtmlElementInfo DataContext
    {
        get;
        set;
    }
}