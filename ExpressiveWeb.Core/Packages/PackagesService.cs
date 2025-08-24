// *********************************************************
// 
// ExpressiveWeb.Core PackagesService.cs
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

using System.Reflection;
using System.Xml;
using ExpressiveWeb.Common.Packages;
using ExpressiveWeb.Common.Packages.Providers;
using ExpressiveWeb.Core.BackgroundServices;

namespace ExpressiveWeb.Core.Packages;

public class PackagesService : IPackagesService
{
    private readonly Dictionary<string, PackageProviderBase> _providers = new();

    private IBackgroundTaskManager _taskManager;

    public PackagesService(IBackgroundTaskManager taskManager)
    {
        _taskManager = taskManager;
    }

    public Task Init()
    {
        return Task.Run(() =>
        {
            // Charger l'assembly PackagesProviders
            Assembly assembly = Assembly.LoadFrom("PackagesProviders.dll");

            // Obtenir tous les types qui ont l'attribut DeclareProviderAttribute
            List<Type> providerTypes = assembly.GetTypes()
                .Where(type => type.GetCustomAttribute<DeclareProviderAttribute>() != null)
                .ToList();

            foreach (Type providerType in providerTypes)
            {
                DeclareProviderAttribute? attribute = providerType.GetCustomAttribute<DeclareProviderAttribute>();
                if (attribute != null)
                {
                    object? instance = Activator.CreateInstance(providerType);
                    if (instance is PackageProviderBase provider)
                    {
                        _providers.Add(attribute.ProviderName, provider);
                    }
                }
            }
        });
    }

    public PackageProviderBase? GetProvider(string providerName)
    {
        return _providers.FirstOrDefault(p => p.Key == providerName).Value;
    }

    private List<WebResource> _webResources = new();

    public async Task LoadPackages(Project.Project project)
    {
        _webResources.Clear();

        foreach (PackageReference packageDefinition in project.PackageReferences)
        {
            var provider = GetProvider(packageDefinition.Provider);
            var webResources = await provider!.GetWebResources(packageDefinition);
            _webResources.AddRange(webResources);
        }
    }

    public PackageReference? ReadPackageFromXmlNode(XmlNode node)
    {
        string pkgName = node.Attributes?["Name"]?.Value ?? string.Empty;
        if (string.IsNullOrEmpty(pkgName))
        {
            return null;
        }

        string pkgVersion = node.Attributes?["Version"]?.Value ?? string.Empty;
        string pkgProvider = node.Attributes?["Provider"]?.Value ?? string.Empty;

        PackageReference reference = new()
        {
            Name = pkgName,
            Version = pkgVersion,
            Provider = pkgProvider
        };

        return reference;
    }
}