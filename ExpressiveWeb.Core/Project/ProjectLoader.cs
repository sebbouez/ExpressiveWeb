// *********************************************************
// 
// ExpressiveWeb.Core ProjectLoader.cs
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

using ExpressiveWeb.Common.Packages.Providers;
using ExpressiveWeb.Core.Kit;
using ExpressiveWeb.Core.Style;
using Microsoft.Extensions.DependencyInjection;

namespace ExpressiveWeb.Core.Project;

public class ProjectLoader
{
    public async Task<Project> Create(Kit.Kit kit, string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException("Path cannot be null or empty", nameof(filePath));
        }

        string? directoryPath = Path.GetDirectoryName(filePath);

        FileInfo fileInfo = new(filePath);
        string fileName = fileInfo.Name;

        Project proj = new()
        {
            FileName = fileName,
            RootPath = directoryPath,
            KitName = kit.Name,
            PackageReferences = new List<PackageReference>(kit.DefaultPackages)
        };

        AppServices.ServicesFactory!.GetService<IProjectService>()!.EnsureDefaultProjectItems(directoryPath);
        await AppServices.ServicesFactory!.GetService<IProjectService>()!.WriteAsync(filePath, proj);

        return proj;
    }

    public async Task<Project> Load(string path)
    {
        Project proj = await AppServices.ServicesFactory!.GetService<IProjectService>()!.ReadAsync(path);
        proj.RefreshItems();

        AppServices.ServicesFactory!.GetService<IProjectService>()!.EnsureDefaultProjectItems(proj.RootPath);

        string styleSheet = Path.Combine(proj.RootPath, ".ew", "styles", "default.css");
        if (File.Exists(styleSheet))
        {
            await AppServices.ServicesFactory!.GetService<IStyleService>()!.ParseStyleSheet(styleSheet);
        }

        Kit.Kit kit = await AppServices.ServicesFactory!.GetService<IKitService>()!.LoadKit(proj.KitName);
        proj.Kit = kit;

        return proj;
    }
}