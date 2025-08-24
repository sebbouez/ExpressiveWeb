using System.Xml.Linq;
using ExpressiveWeb.Common.Packages.Providers;
using ExpressiveWeb.Common.Services;
using ExpressiveWeb.Core.BackgroundServices;
using ExpressiveWeb.Core.Log;
using ExpressiveWeb.Core.Project;
using ExpressiveWeb.Core.Packages;

namespace TestProject;

public class ProjectServiceTests
{
    private sealed class DummyLogService : ILogService
    {
        public void Info(string message) { }
        public void Warning(string message) { }
        public void Error(string message) { }
        public void Error(Exception ex) { }
        public void Dispose() { }
    }

    private sealed class DummyTaskManager : IBackgroundTaskManager
    {
        public List<BackgroundTaskBase> Tasks { get; } = new();
        public event EventHandler<BackgroundTaskManagerStatus>? StatusChanged;
        public void AddTask(BackgroundTaskBase task)
        {
            Tasks.Add(task);
            StatusChanged?.Invoke(this, BackgroundTaskManagerStatus.Running);
        }
    }

    private sealed class DummyPackagesService : IPackagesService
    {
        public Task Init() => Task.CompletedTask;
        public PackageProviderBase? GetProvider(string providerName) => null;
        public Task LoadPackages(Project project) => Task.CompletedTask;
        public PackageReference? ReadPackageFromXmlNode(System.Xml.XmlNode node)
        {
            string name = node.Attributes?["Name"]?.Value ?? string.Empty;
            string version = node.Attributes?["Version"]?.Value ?? string.Empty;
            string provider = node.Attributes?["Provider"]?.Value ?? string.Empty;
            return new PackageReference { Name = name, Version = version, Provider = provider };
        }
    }

    private static string CreateTempDirectory()
    {
        string dir = Path.Combine(Path.GetTempPath(), "EW_Test_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dir);
        return dir;
    }

    [Fact]
    public async Task ReadAsync_ShouldParseValidProjectXml()
    {
        // Arrange
        string tempDir = CreateTempDirectory();
        string projectPath = Path.Combine(tempDir, "myproject.ewp");

        XDocument doc = new(new XElement("Project",
            new XAttribute("Kit", "MyKit"),
            new XElement("Properties",
                new XElement("Title", "Hello"),
                new XElement("Author", "John")
            ),
            new XElement("Packages",
                new XElement("PackageReference",
                    new XAttribute("Name", "Bootstrap"),
                    new XAttribute("Version", "5.3.0"),
                    new XAttribute("Provider", "cdn")
                )
            ),
            new XElement("Services",
                new XElement("ServiceReference",
                    new XAttribute("Name", "MyApi"),
                    new XAttribute("Version", "1.0"),
                    new XAttribute("Provider", "rest")
                )
            )
        ));
        await using (var fs = File.Create(projectPath))
        {
            await doc.SaveAsync(fs, SaveOptions.None, CancellationToken.None);
        }

        ProjectService sut = new(new DummyTaskManager(), new DummyLogService(), new DummyPackagesService());

        // Act
        Project project = await sut.ReadAsync(projectPath);

        // Assert
        Assert.Equal("MyKit", project.KitName);
        Assert.Equal("myproject.ewp", project.FileName);
        Assert.Equal(tempDir, project.RootPath);
        Assert.Equal("Hello", project.Properties["Title"]);
        Assert.Equal("John", project.Properties["Author"]);
        Assert.Single(project.PackageReferences);
        PackageReference pkg = project.PackageReferences[0];
        Assert.Equal("Bootstrap", pkg.Name);
        Assert.Equal("5.3.0", pkg.Version);
        Assert.Equal("cdn", pkg.Provider);
        Assert.Single(project.ServiceReferences);
        ServiceReference svc = project.ServiceReferences[0];
        Assert.Equal("MyApi", svc.Name);
        Assert.Equal("1.0", svc.Version);
        Assert.Equal("rest", svc.Provider);
    }

    [Fact]
    public async Task ReadAsync_ShouldThrow_WhenNoProjectRoot()
    {
        // Arrange
        string tempDir = CreateTempDirectory();
        string projectPath = Path.Combine(tempDir, "invalid.ewp");
        XDocument doc = new(new XElement("Root",
            new XElement("Properties")
        ));
        await using (var fs = File.Create(projectPath))
        {
            await doc.SaveAsync(fs, SaveOptions.None, CancellationToken.None);
        }

        ProjectService sut = new(new DummyTaskManager(), new DummyLogService(), new DummyPackagesService());

        // Act & Assert
        await Assert.ThrowsAsync<InvalidDataException>(() => sut.ReadAsync(projectPath));
    }

    [Fact]
    public void EnsureDefaultProjectItems_ShouldCreateFoldersAndFiles()
    {
        // Arrange
        string tempDir = CreateTempDirectory();
        ProjectService sut = new(new DummyTaskManager(), new DummyLogService(), new DummyPackagesService());

        // Act
        sut.EnsureDefaultProjectItems(tempDir);

        // Assert
        Assert.True(Directory.Exists(Path.Combine(tempDir, ".ew")));
        Assert.True(Directory.Exists(Path.Combine(tempDir, ".ew", "scripts")));
        Assert.True(Directory.Exists(Path.Combine(tempDir, ".ew", "styles")));
        Assert.True(File.Exists(Path.Combine(tempDir, ".ew", "styles", "default.css")));
    }

    [Fact]
    public async Task WriteAsync_ShouldCreateExpectedXml()
    {
        // Arrange
        string tempDir = CreateTempDirectory();
        string projectPath = Path.Combine(tempDir, "out.ewp");

        Project info = new()
        {
            KitName = "KitX",
            FileName = "out.ewp",
            RootPath = tempDir,
            Properties = new Dictionary<string, string>
            {
                ["Foo"] = "Bar",
                ["Baz"] = "Qux"
            },
            PackageReferences = new List<PackageReference>
            {
                new() { Name = "Pkg1", Version = "1.2.3", Provider = "prov" }
            }
        };

        ProjectService sut = new(new DummyTaskManager(), new DummyLogService(), new DummyPackagesService());

        // Act
        await sut.WriteAsync(projectPath, info);

        // Assert
        Assert.True(File.Exists(projectPath));
        XDocument doc = XDocument.Load(projectPath);
        XElement? projectElem = doc.Element("Project");
        Assert.NotNull(projectElem);
        Assert.Equal("KitX", projectElem!.Attribute("Kit")?.Value);
        // Properties
        Assert.Equal("Bar", projectElem.Element("Properties")?.Element("Foo")?.Value);
        Assert.Equal("Qux", projectElem.Element("Properties")?.Element("Baz")?.Value);
        // Packages
        XElement? pkg = projectElem.Element("Packages")?.Element("PackageReference");
        Assert.NotNull(pkg);
        Assert.Equal("Pkg1", pkg!.Attribute("Name")?.Value);
        Assert.Equal("1.2.3", pkg.Attribute("Version")?.Value);
        Assert.Equal("prov", pkg.Attribute("Provider")?.Value);
        // No Services section is written by WriteAsync; ensure it's absent
        Assert.Null(projectElem.Element("Services"));
    }
}
