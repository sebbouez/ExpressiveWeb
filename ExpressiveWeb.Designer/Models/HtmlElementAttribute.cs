using System.Text.Json.Serialization;

namespace ExpressiveWeb.Designer.Models;

public class HtmlElementAttribute
{
    [JsonPropertyName("name")]
    public string Name
    {
        get;
        set;
    } = string.Empty;

    [JsonPropertyName("value")]
    public string Value
    {
        get;
        set;
    } = string.Empty;
}