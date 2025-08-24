using ExpressiveWeb.Core.Env;

namespace TestProject;

public class AppEnvironmentServiceTests
{
    [Fact]
    public void Constructor_ShouldInitializeValidPaths()
    {
        // Act
        AppEnvironmentService env = new();

        // Assert
        Assert.True(env.IsValid);
        Assert.EndsWith("ExpressiveWeb", env.ApplicationDataFolder, StringComparison.OrdinalIgnoreCase);
        Assert.EndsWith("Kits", env.KitsFolderPath, StringComparison.OrdinalIgnoreCase);
        Assert.EndsWith("Library", env.LibraryFolderPath, StringComparison.OrdinalIgnoreCase);
        
        Assert.True(Directory.Exists(env.ApplicationDataFolder));
        Assert.True(Directory.Exists(env.LibraryFolderPath));
    }
}