// *********************************************************
// 
// ExpressiveWeb.Core ProjectService.cs
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
using System.Xml.Linq;
using ExpressiveWeb.Common.Packages.Providers;
using ExpressiveWeb.Common.Services;
using ExpressiveWeb.Core.BackgroundServices;
using ExpressiveWeb.Core.Log;
using ExpressiveWeb.Core.Packages;

namespace ExpressiveWeb.Core.Project;

public class ProjectService : IProjectService
{
    private readonly ILogService _logService;
    private readonly IBackgroundTaskManager _taskManager;
    private readonly IPackagesService _packagesService;

    public ProjectService(IBackgroundTaskManager taskManager, ILogService logService, IPackagesService packagesService)
    {
        _taskManager = taskManager;
        _logService = logService;
        _packagesService = packagesService;
    }

    public async Task<Project> ReadAsync(string xmlFilePath)
    {
        _logService.Info($"Reading project file {xmlFilePath}");

        XmlDocument doc = new();
        await using FileStream stream = File.OpenRead(xmlFilePath);
        doc.Load(stream);

        XmlElement? projectElement = doc.DocumentElement;
        if (projectElement == null || projectElement.Name != "Project")
        {
            _logService.Error($"No <Project> root element found in {xmlFilePath}");
            throw new InvalidDataException("No <Project> root element found.");
        }

        string kit = projectElement.GetAttribute("Kit") ?? "";

        Dictionary<string, string> properties = new();
        XmlNode? propertiesElement = projectElement.SelectSingleNode("Properties");
        if (propertiesElement != null)
        {
            foreach (XmlNode prop in propertiesElement.ChildNodes)
            {
                if (prop.NodeType == XmlNodeType.Element)
                {
                    properties[prop.Name] = prop.InnerText;
                }
            }
        }

        List<PackageReference> packageReferences = new();
        XmlNodeList? pkgsNodes = projectElement.SelectNodes("//Packages/PackageReference");
        if (pkgsNodes != null)
        {
            foreach (XmlNode node in pkgsNodes)
            {
                PackageReference? package = _packagesService.ReadPackageFromXmlNode(node);
                if (package != null)
                {
                    packageReferences.Add(package);
                }
            }
        }

        List<ServiceReference> servicesReferences = new();
        XmlNodeList? servicesNodes = projectElement.SelectNodes("//Services/ServiceReference");
        if (servicesNodes != null)
        {
            foreach (XmlNode p in servicesNodes)
            {
                if (p is XmlElement serviceElement)
                {
                    servicesReferences.Add(new ServiceReference
                    {
                        Name = serviceElement.GetAttribute("Name") ?? "",
                        Version = serviceElement.GetAttribute("Version") ?? "",
                        Provider = serviceElement.GetAttribute("Provider") ?? ""
                    });
                }
            }
        }

        _logService.Info("Read OK");

        return new Project
        {
            KitName = kit,
            FileName = new FileInfo(xmlFilePath).Name,
            RootPath = Directory.GetParent(xmlFilePath)!.FullName,
            Properties = properties,
            PackageReferences = packageReferences,
            ServiceReferences = servicesReferences
        };
    }

    public void EnsureDefaultProjectItems(string projectFolderPath)
    {
        if (!Directory.Exists(Path.Combine(projectFolderPath, ".ew")))
        {
            Directory.CreateDirectory(Path.Combine(projectFolderPath, ".ew"));
        }

        if (!Directory.Exists(Path.Combine(projectFolderPath, ".ew", "scripts")))
        {
            Directory.CreateDirectory(Path.Combine(projectFolderPath, ".ew", "scripts"));
        }

        if (!Directory.Exists(Path.Combine(projectFolderPath, ".ew", "styles")))
        {
            Directory.CreateDirectory(Path.Combine(projectFolderPath, ".ew", "styles"));
        }

        if (!File.Exists(Path.Combine(projectFolderPath, ".ew", "styles", "default.css")))
        {
            using var stream = File.Create(Path.Combine(projectFolderPath, ".ew", "styles", "default.css"));
        }
    }

    public async Task WriteAsync(string xmlFilePath, Project info)
    {
        XElement projectElem = new("Project",
            new XAttribute("Kit", info.KitName ?? string.Empty),
            new XElement("Properties",
                info.Properties.Select(p => new XElement(p.Key, p.Value))
            ),
            new XElement("Packages",
                info.PackageReferences.Select(pkg =>
                    new XElement("PackageReference",
                        new XAttribute("Name", pkg.Name ?? string.Empty),
                        new XAttribute("Version", pkg.Version ?? string.Empty),
                        new XAttribute("Provider", pkg.Provider ?? string.Empty)
                    )
                )
            )
        );

        XDocument doc = new(
            new XText("\n"),
            projectElem,
            new XText("\n")
        );

        await using FileStream stream = File.Create(xmlFilePath);
        await doc.SaveAsync(stream, SaveOptions.None, CancellationToken.None);
    }
}