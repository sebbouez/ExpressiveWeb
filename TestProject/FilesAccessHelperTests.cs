using ExpressiveWeb.Core.FileManagement;

namespace TestProject;

public class FilesAccessHelperTests
{
    private static string CreateTempPath(string? suffix = null)
    {
        string baseDir = Path.Combine(Path.GetTempPath(), "EW_Test_" + Guid.NewGuid().ToString("N"));
        if (!string.IsNullOrEmpty(suffix))
        {
            baseDir = Path.Combine(baseDir, suffix);
        }
        return baseDir;
    }

    [Fact]
    public async Task WriteAllTextAsync_ShouldCreateDirectoryAndWrite()
    {
        string dir = CreateTempPath();
        string file = Path.Combine(dir, "sub", "file.txt");

        bool ok = await FilesAccessHelper.WriteAllTextAsync(file, "hello");
        Assert.True(ok);
        Assert.True(File.Exists(file));
        string content = await File.ReadAllTextAsync(file);
        Assert.Equal("hello", content);
    }

    [Fact]
    public void WriteAllText_ShouldCreateDirectoryAndWrite()
    {
        string dir = CreateTempPath();
        string file = Path.Combine(dir, "sub2", "sync.txt");

        bool ok = FilesAccessHelper.WriteAllText(file, "world");
        Assert.True(ok);
        Assert.True(File.Exists(file));
        string content = File.ReadAllText(file);
        Assert.Equal("world", content);
    }

    [Fact]
    public void ReadAllText_ShouldReturnContent_OrThrowWhenMissing()
    {
        string dir = CreateTempPath();
        Directory.CreateDirectory(dir);
        string file = Path.Combine(dir, "r.txt");
        File.WriteAllText(file, "abc");

        string read = FilesAccessHelper.ReadAllText(file);
        Assert.Equal("abc", read);

        // Missing file should throw IOException
        string missing = Path.Combine(dir, "missing.txt");
        Assert.Throws<IOException>(() => FilesAccessHelper.ReadAllText(missing));
    }
}
