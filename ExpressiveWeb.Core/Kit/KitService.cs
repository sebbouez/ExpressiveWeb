// *********************************************************
// 
// ExpressiveWeb.Core KitService.cs
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

using System.Xml;
using System.Xml.Serialization;
using ExpressiveWeb.Common.Packages.Providers;
using ExpressiveWeb.Core.Env;
using ExpressiveWeb.Core.FileManagement;
using ExpressiveWeb.Core.Kit.ComponentFeatures;
using ExpressiveWeb.Core.Log;
using ExpressiveWeb.Core.Packages;
using HtmlAgilityPack;

namespace ExpressiveWeb.Core.Kit;

public class KitService : IKitService
{
    private readonly IEnvironmentService _environmentService;

    // TODO : Find a better way to do this that is compatible with AOT
    private readonly Dictionary<string, Type> _knownFeatures = new()
    {
        {"InlineEdit", typeof(InlineEditFeature)},
        {"SwapTextTag", typeof(SwapTextTagFeature)},
        {"ContextualBoxStyle", typeof(ContextualBoxStyleFeature)}
    };

    private readonly ILogService _logService;
    private readonly IPackagesService _packagesService;

    public KitService(IEnvironmentService environmentService, ILogService logService, IPackagesService packagesService)
    {
        _packagesService = packagesService;
        _logService = logService;
        _environmentService = environmentService;
    }

    public Task<List<Kit>> LoadKits()
    {
        return Task.Run(async () =>
        {
            List<Kit> result = new();

            try
            {
                string root = _environmentService.KitsFolderPath;
                if (string.IsNullOrWhiteSpace(root) || !Directory.Exists(root))
                {
                    return result;
                }

                foreach (string kitDir in Directory.GetDirectories(root))
                {
                    string kitName = Path.GetFileName(kitDir);

                    try
                    {
                        Kit kit = await LoadKit(kitName);
                        result.Add(kit);
                    }
                    catch (Exception ex)
                    {
                        _logService.Error(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                _logService.Error(ex);
            }

            return result;
        });
    }

    public Task<Kit?> LoadKit(string kitName)
    {
        return Task.Run(() =>
        {
            string kitXmlFile = Path.Combine(_environmentService.KitsFolderPath, kitName, "kit.xml");

            if (!File.Exists(kitXmlFile))
            {
                return null;
            }

            Kit? result = ReadKitXml(kitXmlFile);

            if (result == null)
            {
                return null;
            }

            LoadKitComponents(kitName, result);

            return result;
        });
    }

    public Kit? ReadKitXml(string xmlFilePath)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(xmlFilePath) || !File.Exists(xmlFilePath))
            {
                return null;
            }

            XmlDocument doc = new();
            doc.Load(xmlFilePath);

            XmlElement? root = doc.DocumentElement;
            if (root == null)
            {
                return null;
            }

            string directoryName = new DirectoryInfo(Path.GetDirectoryName(xmlFilePath) ?? string.Empty).Name;

            Kit kit = new(directoryName);

            string displayName = root.SelectSingleNode("Name")?.InnerText ?? "Unknown";
            kit.DisplayName = displayName;

            string? versionText = root.SelectSingleNode("Version")?.InnerText;
            if (!string.IsNullOrWhiteSpace(versionText) && Version.TryParse(versionText, out Version? ver))
            {
                kit.Version = ver;
            }

            XmlNodeList? pkgs = root.SelectNodes("//DefaultPackages/PackageReference");
            if (pkgs != null)
            {
                foreach (XmlNode node in pkgs)
                {
                    PackageReference? package = _packagesService.ReadPackageFromXmlNode(node);
                    if (package != null)
                    {
                        kit.DefaultPackages.Add(package);
                    }
                }
            }

            XmlNodeList? templates = root.SelectNodes("//Templates/Template");
            if (templates != null)
            {
                foreach (XmlNode node in templates)
                {
                    KitPageTemplate? template = ReadTemplateFromXmlNode(node);
                    if (template != null)
                    {
                        kit.Templates.Add(template);
                    }
                }
            }

            return kit;
        }
        catch (Exception ex)
        {
            _logService.Error(ex);
            return null;
        }
    }

    public bool CopyTemplateToFile(string kitName, KitPageTemplate template, string targetFileName)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(template.File) || string.IsNullOrWhiteSpace(targetFileName))
            {
                return false;
            }

            // Construire le chemin complet du fichier template source
            string templateSourcePath = Path.Combine(_environmentService.KitsFolderPath, kitName, "templates", template.File);

            // Vérifier que le fichier template existe
            if (!File.Exists(templateSourcePath))
            {
                return false;
            }

            // Utiliser la méthode CopyFile de FilesAccessHelper
            return FilesAccessHelper.CopyFile(templateSourcePath, targetFileName);
        }
        catch (Exception ex)
        {
            _logService.Error(ex);
            return false;
        }
    }

    private KitPageTemplate? ReadTemplateFromXmlNode(XmlNode node)
    {
        KitPageTemplate pageTemplate = new()
        {
            Name = node.Attributes?["Name"]?.Value ?? string.Empty,
            File = node.Attributes?["File"]?.Value ?? string.Empty
        };
        return pageTemplate;
    }

    private static bool DetectComponentConfig(KitComponent component)
    {
        try
        {
            HtmlDocument doc = new();
            doc.LoadHtml(component.Template);

            HtmlNode? relevantNode = doc.DocumentNode.ChildNodes.FirstOrDefault(x => x is not HtmlTextNode);
            if (relevantNode == null)
            {
                return false;
            }

            component.HtmlTagName = relevantNode.Name;
            component.HtmlClassName = relevantNode.GetAttributeValue("class", "");

            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    private void LoadKitComponents(string kitName, Kit result)
    {
        string componentsFolderPath = Path.Combine(_environmentService.KitsFolderPath, kitName, "components");

        IEnumerable<string> xmlFiles = Directory.GetFiles(componentsFolderPath, "*.xml", SearchOption.AllDirectories);

        foreach (string file in xmlFiles)
        {
            try
            {
                KitComponent? component = ReadKitComponentXml(file);

                if (component == null)
                {
                    continue;
                }

                if (!DetectComponentConfig(component))
                {
                    continue;
                }

                result.Components.Add(component);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

    private KitComponent? ReadKitComponentXml(string xmlFilePath)
    {
        XmlDocument doc = new();
        doc.Load(xmlFilePath);

        KitComponent component = new();
        XmlElement? root = doc.DocumentElement;

        if (root != null && root.SelectSingleNode("UID") != null)
        {
            component.UID = root.SelectSingleNode("UID")!.InnerText;
            component.Name = root.SelectSingleNode("Name")?.InnerText ?? string.Empty;
            component.Family = root.SelectSingleNode("Family")?.InnerText ?? string.Empty;
            component.Accepts = root.SelectSingleNode("Allows")?.InnerText ?? string.Empty;
            component.Denies = root.SelectSingleNode("Denies")?.InnerText ?? string.Empty;
            component.Slots = root.SelectSingleNode("Slots")?.InnerText ?? string.Empty;
            component.Template = root.SelectSingleNode("Template")?.InnerText ?? string.Empty;

            XmlNodeList? featuresNodes = root.SelectNodes("//Features/*");
            if (featuresNodes != null)
            {
                foreach (XmlNode featureNode in featuresNodes)
                {
                    if (!_knownFeatures.TryGetValue(featureNode.Name, out Type? featureType))
                    {
                        continue;
                    }

                    ComponentFeatureBase? feature = (ComponentFeatureBase?) Activator.CreateInstance(featureType);
                    if (feature != null)
                    {
                        feature.Init(featureNode);
                        component.Features.Add(feature);
                    }
                }
            }

            XmlNodeList? actions = root.SelectNodes("//Actions/Action");
            if (actions == null)
            {
                return component;
            }

            foreach (XmlNode actionNode in actions)
            {
                QuickAction action = new()
                {
                    Header = actionNode.Attributes?["Header"]?.Value ?? string.Empty,
                    Command = actionNode.Attributes?["Command"]?.Value ?? string.Empty,
                    Params = actionNode.Attributes?["Params"]?.Value ?? string.Empty
                };
                component.ActionList.Add(action);
            }

            return component;
        }

        return null;
    }

    public static void SerializeToXml(KitComponent component, string xmlFilePath)
    {
        XmlSerializer serializer = new(typeof(KitComponent));
        using StreamWriter writer = new(xmlFilePath);
        serializer.Serialize(writer, component);
    }
}