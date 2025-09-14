using System.Diagnostics;
using System.Reflection;

namespace ExpressiveWeb.Designer;

internal static class ResourcesHelper
{
    internal const string RESOURCES_ROOT_NAMESPACE = "ExpressiveWeb.Designer.Resources";
    
    internal static string ResolveResource(string resourceName)
    {
        Stopwatch sw = new();
        sw.Start();

        Assembly assembly = Assembly.GetExecutingAssembly();

        string? foundResource = assembly.GetManifestResourceNames().FirstOrDefault(x => x.StartsWith(ResourcesHelper.RESOURCES_ROOT_NAMESPACE, StringComparison.InvariantCultureIgnoreCase)
                                                                                        && x.EndsWith(resourceName, StringComparison.OrdinalIgnoreCase));

        if (foundResource == null)
        {
            return string.Empty;
        }

        using Stream? stream = assembly.GetManifestResourceStream(foundResource);
        if (stream == null)
        {
            return "";
        }

        sw.Stop();
        return new StreamReader(stream).ReadToEnd();
    }
}