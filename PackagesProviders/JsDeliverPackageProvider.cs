// *********************************************************
// 
// ExpressiveWeb.Core JsDeliverPackageProvider.cs
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

using System.Text.Json;
using ExpressiveWeb.Common.Packages;
using ExpressiveWeb.Common.Packages.Providers;
using PackagesProviders.JsDelivr;

namespace PackagesProviders;

[DeclareProvider("JsDeliver")]
public class JsDeliverPackageProvider : PackageProviderBase
{
    public override Task<List<WebResource>> GetWebResources(PackageReference reference)
    {
        throw new NotImplementedException();
    }

    public override async Task<List<PackageReference>> GetPackages(string name)
    {
        HttpClient client = new();
        var response = await client.GetAsync(string.Concat("https://data.jsdelivr.com/v1/packages/npm/", name));
        var content = await response.Content.ReadAsStringAsync();

        JsonSerializer.Deserialize<PackageInfo>(content);

        return new List<PackageReference>();
    }
}