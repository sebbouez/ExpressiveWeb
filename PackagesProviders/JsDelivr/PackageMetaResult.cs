// *********************************************************
// 
// PackagesProviders PackageMetaResult.cs
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

namespace PackagesProviders.JsDelivr;

using System.Collections.Generic;
using System.Text.Json.Serialization;

public class PackageInfo
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;
    
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonPropertyName("tags")]
    public Tags Tags { get; set; } = new();
    
    [JsonPropertyName("versions")]
    public List<Version> Versions { get; set; } = new();
    
    [JsonPropertyName("links")]
    public PackageLinks Links { get; set; } = new();
}

public class Tags
{
    [JsonPropertyName("beta")]
    public string Beta { get; set; } = string.Empty;
    
    [JsonPropertyName("latest")]
    public string Latest { get; set; } = string.Empty;
}

public class Version
{
    [JsonPropertyName("version")]
    public string VersionNumber { get; set; } = string.Empty;
    
    [JsonPropertyName("links")]
    public VersionLinks Links { get; set; } = new();
}

public class VersionLinks
{
    [JsonPropertyName("self")]
    public string Self { get; set; } = string.Empty;
    
    [JsonPropertyName("entrypoints")]
    public string Entrypoints { get; set; } = string.Empty;
    
    [JsonPropertyName("stats")]
    public string Stats { get; set; } = string.Empty;
}

public class PackageLinks
{
    [JsonPropertyName("stats")]
    public string Stats { get; set; } = string.Empty;
}