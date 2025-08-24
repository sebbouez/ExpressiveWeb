using System.Xml.Linq;
using ExpressiveWeb.Core.Env;
using ExpressiveWeb.Core.Libraries;
using ExpressiveWeb.Core.Log;

namespace TestProject;

public class LibraryServiceTests
{
    private sealed class TempEnvironmentService : IEnvironmentService
    {
        public bool IsValid { get; } = true;
        public string ApplicationDataFolder { get; init; } = Path.Combine(Path.GetTempPath(), "EW_App_" + Guid.NewGuid().ToString("N"));
        public string KitsFolderPath { get; init; } = Path.Combine(Path.GetTempPath(), "EW_Kits_" + Guid.NewGuid().ToString("N"));
        public string LibraryFolderPath { get; init; } = Path.Combine(Path.GetTempPath(), "EW_Lib_" + Guid.NewGuid().ToString("N"));
    }

    private sealed class TestLogService : ILogService
    {
        public readonly List<string> Infos = new();
        public readonly List<string> Warnings = new();
        public readonly List<string> Errors = new();
        public Exception? LastException;
        public void Info(string message) => Infos.Add(message);
        public void Warning(string message) => Warnings.Add(message);
        public void Error(string message) => Errors.Add(message);
        public void Error(Exception ex)
        {
            LastException = ex;
            Errors.Add(ex.Message);
        }
        public void Dispose() { }
    }

    private static async Task<string> WriteXmlAsync(string dir, string fileName, XDocument doc)
    {
        Directory.CreateDirectory(dir);
        string path = Path.Combine(dir, fileName);
        await using (var fs = File.Create(path))
        {
            await doc.SaveAsync(fs, SaveOptions.None, CancellationToken.None);
        }
        return path;
    }

    [Fact]
    public void GetItemTypeForCategory_ShouldReturnExpectedTypes()
    {
        // Arrange
        var env = new TempEnvironmentService();
        var log = new TestLogService();
        LibraryService sut = new(log, env);

        // Act & Assert
        Assert.Equal(typeof(LibraryStyleItem), sut.GetItemTypeForCategory(CategoryNodeType.Style));
        Assert.Equal(typeof(LibraryPaletteItem), sut.GetItemTypeForCategory(CategoryNodeType.Palette));
        Assert.Equal(typeof(LibraryImageItem), sut.GetItemTypeForCategory(CategoryNodeType.Image));
        Assert.Null(sut.GetItemTypeForCategory(CategoryNodeType.None));
    }

    [Fact]
    public void GetItemsByCategory_ShouldFilterByMappedType()
    {
        // Arrange
        var env = new TempEnvironmentService();
        var log = new TestLogService();
        LibraryService sut = new(log, env)
        {
            LibraryItems =
            [
                new LibraryStyleItem(),
                new LibraryPaletteItem(),
                new LibraryImageItem(),
                new LibraryStyleItem()
            ]
        };

        // Act
        var styles = sut.GetItemsByCategory(CategoryNodeType.Style).ToList();
        var palettes = sut.GetItemsByCategory(CategoryNodeType.Palette).ToList();
        var images = sut.GetItemsByCategory(CategoryNodeType.Image).ToList();
        var none = sut.GetItemsByCategory(CategoryNodeType.None).ToList();

        // Assert
        Assert.Equal(2, styles.Count);
        Assert.Single(palettes);
        Assert.Single(images);
        Assert.Empty(none);
    }

    [Fact]
    public async Task LoadItemsAsync_ShouldLoadValidLibraryItems()
    {
        // Arrange
        var env = new TempEnvironmentService();
        var log = new TestLogService();
        Directory.CreateDirectory(env.LibraryFolderPath);

        // Style item XML
        await WriteXmlAsync(env.LibraryFolderPath, "style1.xml", new XDocument(
            new XElement("LibraryItem",
                new XElement("Type", "style"),
                new XElement("Name", "Primary Button"),
                new XElement("CssClass",
                    new XAttribute("Name", ".btn-primary"),
                    new XCData(".btn-primary{color:red;}")
                )
            )));

        // Palette item XML
        await WriteXmlAsync(env.LibraryFolderPath, "palette1.xml", new XDocument(
            new XElement("LibraryItem",
                new XElement("Type", "palette"),
                new XElement("Name", "Default Palette"),
                new XElement("Colors",
                    new XElement("Color",
                        new XAttribute("Name", "Primary"),
                        new XAttribute("Value", "#ff0000"),
                        new XAttribute("VarName", "--primary")
                    )
                )
            )));

        // Image item XML
        await WriteXmlAsync(env.LibraryFolderPath, "image1.xml", new XDocument(
            new XElement("LibraryItem",
                new XElement("Type", "image"),
                new XElement("Name", "Logo"),
                new XElement("File", new XAttribute("Name", "logo.png"))
            )));

        LibraryService sut = new(log, env);

        // Act
        await sut.LoadItemsAsync();

        // Assert
        Assert.Equal(3, sut.LibraryItems.Count);
        // Validate Style
        var style = sut.LibraryItems.OfType<LibraryStyleItem>().FirstOrDefault();
        Assert.NotNull(style);
        Assert.Equal("Primary Button", style!.Name);
        Assert.Equal(".btn-primary", style.CssClassName);
        Assert.Contains(".btn-primary{color:red;}", style.CssContent);
        // Validate Palette
        var palette = sut.LibraryItems.OfType<LibraryPaletteItem>().FirstOrDefault();
        Assert.NotNull(palette);
        Assert.Equal("Default Palette", palette!.Name);
        Assert.Single(palette.Colors);
        Assert.Equal("Primary", palette.Colors[0].Name);
        Assert.Equal("#ff0000", palette.Colors[0].Value);
        Assert.Equal("--primary", palette.Colors[0].VarName);
        // Validate Image
        var image = sut.LibraryItems.OfType<LibraryImageItem>().FirstOrDefault();
        Assert.NotNull(image);
        Assert.Equal("Logo", image!.Name);
        Assert.Equal("logo.png", image.FileName);

        // No errors expected
        Assert.Empty(log.Errors);
    }

    [Fact]
    public async Task LoadItemsAsync_ShouldHandleMissingAndInvalidFiles()
    {
        // Arrange: missing folder
        var envMissing = new TempEnvironmentService();
        var logMissing = new TestLogService();
        LibraryService sutMissing = new(logMissing, envMissing);

        // Act
        await sutMissing.LoadItemsAsync();

        // Assert
        Assert.Empty(sutMissing.LibraryItems);
        Assert.Contains(logMissing.Warnings, w => w.Contains("Library folder not found"));

        // Arrange: invalid/mixed files
        var env = new TempEnvironmentService();
        var log = new TestLogService();
        Directory.CreateDirectory(env.LibraryFolderPath);

        // Missing <Type>
        await WriteXmlAsync(env.LibraryFolderPath, "no_type.xml", new XDocument(
            new XElement("LibraryItem",
                new XElement("Name", "NoType")
            )));

        // Unknown type
        await WriteXmlAsync(env.LibraryFolderPath, "unknown_type.xml", new XDocument(
            new XElement("LibraryItem",
                new XElement("Type", "unknown"),
                new XElement("Name", "UnknownType")
            )));

        // Malformed XML: write broken content
        string brokenPath = Path.Combine(env.LibraryFolderPath, "broken.xml");
        await File.WriteAllTextAsync(brokenPath, "<LibraryItem><Type>style</Type><Name>Bad");

        LibraryService sut = new(log, env);

        // Act
        await sut.LoadItemsAsync();

        // Assert: All skipped or errored, nothing loaded
        Assert.Empty(sut.LibraryItems);
        // Warnings for missing/unknown type
        Assert.Contains(log.Warnings, w => w.Contains("missing <Type>"));
        Assert.Contains(log.Warnings, w => w.Contains("Unknown library item type"));
        // Error should be logged for malformed XML
        Assert.True(log.Errors.Count >= 1);
    }
}
