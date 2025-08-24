using ExpressiveWeb.Core.Env;
using ExpressiveWeb.Core.Kit;
using ExpressiveWeb.Core.Log;

namespace TestProject;

public class KitServiceTests
{
    private sealed class TempEnvironmentService : IEnvironmentService
    {
        public bool IsValid { get; } = true;
        public string ApplicationDataFolder { get; init; } = Path.Combine(Path.GetTempPath(), "EW_App_" + Guid.NewGuid().ToString("N"));
        public string KitsFolderPath { get; init; } = Path.Combine(Path.GetTempPath(), "EW_Kits_" + Guid.NewGuid().ToString("N"));
        public string LibraryFolderPath { get; init; } = Path.Combine(Path.GetTempPath(), "EW_Lib_" + Guid.NewGuid().ToString("N"));
    }

    private sealed class TempLogService : ILogService
    {
        public void Info(string message) { }
        public void Warning(string message) { }
        public void Error(string message) { }
        public void Error(Exception ex) { }
        public void Dispose() { }
    }

    private sealed class TempPackagesService : ExpressiveWeb.Core.Packages.IPackagesService
    {
        public Task Init() => Task.CompletedTask;
        public ExpressiveWeb.Common.Packages.Providers.PackageProviderBase? GetProvider(string providerName) => null;
        public Task LoadPackages(ExpressiveWeb.Core.Project.Project project) => Task.CompletedTask;
        public ExpressiveWeb.Common.Packages.Providers.PackageReference? ReadPackageFromXmlNode(System.Xml.XmlNode node)
        {
            string name = node.Attributes?["Name"]?.Value ?? string.Empty;
            string version = node.Attributes?["Version"]?.Value ?? string.Empty;
            string provider = node.Attributes?["Provider"]?.Value ?? string.Empty;
            return new ExpressiveWeb.Common.Packages.Providers.PackageReference { Name = name, Version = version, Provider = provider };
        }
    }

    private static string CreateComponentXml(string folder)
    {
        Directory.CreateDirectory(folder);
        string file = Path.Combine(folder, "button.xml");
        string xml = @"<KitComponent>
  <UID>comp-1</UID>
  <Name>Button</Name>
  <Family>Controls</Family>
  <Allows></Allows>
  <Denies></Denies>
  <Slots></Slots>
  <Template>&lt;button class='primary'&gt;Click&lt;/button&gt;</Template>
  <Actions>
    <Action Header='Do' Command='DoCmd' OfType='All' />
  </Actions>
</KitComponent>";
        File.WriteAllText(file, xml);
        return file;
    }

    [Fact]
    public async Task LoadKit_ShouldLoadComponentsFromXml()
    {
        // Arrange
        var env = new TempEnvironmentService();
        string kitName = "MyKit";
        string componentsDir = Path.Combine(env.KitsFolderPath, kitName, "components");
        CreateComponentXml(componentsDir);
        // Create minimal kit.xml expected by KitService
        string kitXmlPath = Path.Combine(env.KitsFolderPath, kitName, "kit.xml");
        Directory.CreateDirectory(Path.GetDirectoryName(kitXmlPath)!);
        await File.WriteAllTextAsync(kitXmlPath, "<Kit><Name>MyKit</Name></Kit>");

        KitService sut = new(env, new TempLogService(), new TempPackagesService());

        // Act
        Kit kit = await sut.LoadKit(kitName);

        // Assert
        Assert.Equal(kitName, kit.Name);
        Assert.Single(kit.Components);
        KitComponent comp = kit.Components[0];
        Assert.Equal("comp-1", comp.UID);
        Assert.Equal("Button", comp.Name);
        Assert.Equal("Controls", comp.Family);
        // From Template detection
        Assert.Equal("button", comp.HtmlTagName);
        Assert.Equal("primary", comp.HtmlClassName);
        // Features are optional in this test XML
        Assert.True(comp.Features.Count >= 0);
        // Actions parsed
        Assert.Single(comp.ActionList);
        Assert.Equal("Do", comp.ActionList[0].Header);
        Assert.Equal("DoCmd", comp.ActionList[0].Command);
    }
}
