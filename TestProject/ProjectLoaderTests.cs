using ExpressiveWeb.Core;
using ExpressiveWeb.Core.Kit;
using ExpressiveWeb.Core.Project;
using ExpressiveWeb.Core.Style;
using Microsoft.Extensions.DependencyInjection;

namespace TestProject;

public class ProjectLoaderTests
{
    private sealed class FakeProjectService : IProjectService
    {
        public string? LastReadPath { get; private set; }
        public string? EnsureDefaultsFor { get; private set; }
        public Project? ReturnedProject { get; set; }

        public Task<Project> ReadAsync(string xmlFilePath)
        {
            LastReadPath = xmlFilePath;
            return Task.FromResult(ReturnedProject!);
        }

        public Task WriteAsync(string xmlFilePath, Project info)
        {
            throw new NotImplementedException();
        }

        public void EnsureDefaultProjectItems(string projectFolderPath)
        {
            EnsureDefaultsFor = projectFolderPath;
        }
    }

    private sealed class FakeStyleService : IStyleService
    {
        public string? LastParsedPath { get; private set; }
        public int ParseCalls { get; private set; }

        public CssStyle ParseStyleAttribute(string style)
        {
            throw new NotImplementedException();
        }

        public Task<ExCSS.Stylesheet> ParseStyleSheet(string styleSheetFilePath)
        {
            LastParsedPath = styleSheetFilePath;
            ParseCalls++;
            // Return null as the loader does not use the stylesheet instance in this context
            return Task.FromResult<ExCSS.Stylesheet>(null!);
        }
    }

    private sealed class FakeKitService : IKitService
    {
        public string? LastKitName { get; private set; }
        public Task<Kit?> LoadKit(string kitName)
        {
            LastKitName = kitName;
            return Task.FromResult(new Kit(kitName));
        }

        public Task<List<Kit>> LoadKits()
        {
            throw new NotImplementedException();
        }

        public Kit? ReadKitXml(string xmlFilePath)
        {
            // Minimal fake: infer kit name from folder name of kit.xml
            // This is sufficient for ProjectLoader tests which don't use this directly.
            try
            {
                if (string.IsNullOrWhiteSpace(xmlFilePath)) return null;
                string? dir = Path.GetDirectoryName(xmlFilePath);
                if (string.IsNullOrEmpty(dir)) return null;
                string kitName = new DirectoryInfo(dir).Name;
                return new Kit(kitName);
            }
            catch
            {
                return null;
            }
        }

        public bool CopyTemplateToFile(string kitName, KitPageTemplate template, string targetFileName)
        {
            throw new NotImplementedException();
        }
    }

    private static string CreateTempDir()
    {
        string dir = Path.Combine(Path.GetTempPath(), "EW_PL_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dir);
        return dir;
    }

    private static void SetupServices(FakeProjectService fps, FakeStyleService fss, FakeKitService fks)
    {
        ServiceCollection sc = new();
        sc.AddSingleton<IProjectService>(fps);
        sc.AddSingleton<IStyleService>(fss);
        sc.AddSingleton<IKitService>(fks);
        AppServices.ServicesFactory = sc.BuildServiceProvider();
    }

    [Fact]
    public async Task Load_ShouldReadProject_EnsureDefaults_ParseStyles_AndLoadKit()
    {
        // Arrange
        string tempDir = CreateTempDir();
        string projectPath = Path.Combine(tempDir, "myproj.ewp");
        // Create expected stylesheet so ParseStyleSheet is called
        string stylesDir = Path.Combine(tempDir, ".ew", "styles");
        Directory.CreateDirectory(stylesDir);
        string defaultCss = Path.Combine(stylesDir, "default.css");
        await File.WriteAllTextAsync(defaultCss, "/* test */");

        FakeProjectService fps = new();
        FakeStyleService fss = new();
        FakeKitService fks = new();
        SetupServices(fps, fss, fks);

        // Project returned by ReadAsync
        Project proj = new()
        {
            FileName = Path.GetFileName(projectPath),
            RootPath = tempDir,
            KitName = "MyKit"
        };
        fps.ReturnedProject = proj;

        ProjectLoader sut = new();

        // Act
        Project loaded = await sut.Load(projectPath);

        // Assert
        Assert.Same(proj, loaded);
        Assert.Equal(projectPath, fps.LastReadPath);
        Assert.Equal(tempDir, fps.EnsureDefaultsFor);
        Assert.Equal(defaultCss, fss.LastParsedPath);
        Assert.Equal(1, fss.ParseCalls);
        Assert.Equal("MyKit", fks.LastKitName);
        Assert.NotNull(loaded.Kit);
        Assert.Equal("MyKit", loaded.Kit.Name);

        // RefreshItems should have populated Items tree (at least root + Inventaire)
        Assert.True(loaded.Items.Count >= 1);
        Assert.Equal(Path.GetFileName(projectPath), loaded.Items[0].Name);
    }

    [Fact]
    public async Task Load_ShouldNotParseStyles_WhenDefaultCssMissing()
    {
        // Arrange
        string tempDir = CreateTempDir();
        string projectPath = Path.Combine(tempDir, "myproj2.ewp");

        FakeProjectService fps = new();
        FakeStyleService fss = new();
        FakeKitService fks = new();
        SetupServices(fps, fss, fks);

        Project proj = new()
        {
            FileName = Path.GetFileName(projectPath),
            RootPath = tempDir,
            KitName = "KitX"
        };
        fps.ReturnedProject = proj;

        ProjectLoader sut = new();

        // Act
        Project loaded = await sut.Load(projectPath);

        // Assert
        Assert.Same(proj, loaded);
        Assert.Null(fss.LastParsedPath);
        Assert.Equal(0, fss.ParseCalls);
        Assert.Equal("KitX", fks.LastKitName);
    }
}
