// *********************************************************
// 
// ExpressiveWeb.Core Project.cs
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

using System.Collections.ObjectModel;
using ExpressiveWeb.Common.Packages.Providers;
using ExpressiveWeb.Common.Services;
using ExpressiveWeb.Core.Style;

namespace ExpressiveWeb.Core.Project;

public class Project
{
    private const string RESERVED_FOLDER_NAME = ".ew";

    private static readonly Dictionary<string, ProjectItemType> ProjectFolderTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        {"styles", ProjectItemType.StyleSheet},
        {"scripts", ProjectItemType.Script},
        {"images", ProjectItemType.Image},
        {"master", ProjectItemType.MasterPage}
    };

    /// <summary>
    ///     Gets or sets the list of items that compose the project.
    ///     They are shown in the Explorer panel.
    /// </summary>
    public ObservableCollection<ProjectItem> Items
    {
        get;
        set;
    } = new();

    /// <summary>
    ///     Gets or sets the filename of the project. For example "myproject.ewp".
    /// </summary>
    public string FileName
    {
        get;
        set;
    } = null!;

    /// <summary>
    ///     Gets or sets the root path of the project on the computer.
    /// </summary>
    public string RootPath
    {
        get;
        set;
    } = null!;

    /// <summary>
    ///     Gets or sets the name of the selected Kit to build the project.
    /// </summary>
    public string KitName
    {
        get;
        set;
    } = "";

    /// <summary>
    ///     Gets the Kit object that is used in the project.
    /// </summary>
    public Kit.Kit Kit
    {
        get;
        internal set;
    }

    /// <summary>
    ///     Gets or sets the list of properties used to configure the project.
    ///     The properties are stored in the project file.
    /// </summary>
    public Dictionary<string, string> Properties
    {
        get;
        set;
    } = new();

    /// <summary>
    ///     Gets or sets the list of packages used in the project.
    ///     For example, JQuery, Boostrap...
    /// </summary>
    public List<PackageReference> PackageReferences
    {
        get;
        set;
    } = new();

    public List<UserStyle> UserStyles
    {
        get;
        set;
    } = new();

    /// <summary>
    ///     Gets or sets the list of services references used in the project.
    ///     It can be APIs used to post or retreive data.
    /// </summary>
    public List<ServiceReference> ServiceReferences
    {
        get;
        set;
    } = new();

    private List<ProjectItem> BuildFileTree()
    {
        List<ProjectItem> items = new();

        if (!Directory.Exists(RootPath))
        {
            return items;
        }

        DirectoryInfo rootDir = new(RootPath);
        foreach (ProjectItem item in BuildFileTreeRecursive(rootDir))
        {
            items.Add(item);
        }

        return items;
    }

    private List<ProjectItem> BuildFileTreeRecursive(DirectoryInfo directory)
    {
        List<ProjectItem> items = new();
        foreach (DirectoryInfo dir in directory.GetDirectories())
        {
            ProjectItem item = new()
            {
                Name = dir.Name,
                ItemType = ProjectItemType.Folder,
                Path = dir.FullName
            };
            List<ProjectItem> children = BuildFileTreeRecursive(dir);
            foreach (ProjectItem child in children)
            {
                item.Children.Add(child);
            }

            items.Add(item);
        }

        foreach (FileInfo file in directory.GetFiles().Where(x => !x.Name.Equals(FileName, StringComparison.OrdinalIgnoreCase)))
        {
            ProjectItemType itemType = GetItemTypeByDirectory(directory);

            items.Add(new ProjectItem
            {
                Name = file.Name,
                ItemType = itemType,
                Path = file.FullName
            });
        }

        return items;
    }

    private static void BuildReservedFolder(ProjectItemType itemType, List<ProjectItem> foundItems, ProjectItem rootItem, string folderDisplayName)
    {
        List<ProjectItem> filteredItems = GetAllItemsRecursively(foundItems)
            .Where(x => x.ItemType.Equals(itemType))
            .ToList();

        ProjectItem localFolderItem = new()
        {
            Name = folderDisplayName,
            ItemType = ProjectItemType.ReservedFolder
        };

        foreach (ProjectItem item in filteredItems)
        {
            localFolderItem.Children.Add(new ProjectItem
            {
                Name = item.Name,
                Path = item.Path,
                ItemType = itemType
            });

            foundItems.Remove(item);
        }

        rootItem.Children.Add(localFolderItem);
    }

    private static IEnumerable<ProjectItem> GetAllItemsRecursively(IEnumerable<ProjectItem> items)
    {
        return items.SelectMany(item => new[] {item}
            .Concat(GetAllItemsRecursively(item.Children)));
    }

    private ProjectItemType GetItemTypeByDirectory(DirectoryInfo directory)
    {
        string relativePath = GetRelativePath(directory);

        foreach (KeyValuePair<string, ProjectItemType> mapping in ProjectFolderTypes)
        {
            if (relativePath.StartsWith(string.Concat(RESERVED_FOLDER_NAME, Path.DirectorySeparatorChar, mapping.Key), StringComparison.OrdinalIgnoreCase))
            {
                return mapping.Value;
            }
        }

        return ProjectItemType.File;
    }

    private string GetRelativePath(DirectoryInfo directory)
    {
        return directory.FullName.Replace(string.Concat(RootPath, Path.DirectorySeparatorChar), "").Trim(Path.DirectorySeparatorChar);
    }

    public void RefreshItems()
    {
        Items.Clear();

        ProjectItem rootItem = new()
        {
            Name = FileName,
            ItemType = ProjectItemType.Project
        };

        Items.Add(rootItem);

        ProjectItem projectReserved = new()
        {
            Name = "Inventaire",
            ItemType = ProjectItemType.ReservedFolder
        };
        rootItem.Children.Add(projectReserved);

        List<ProjectItem> foundItems = BuildFileTree();

        BuildReferencesFolder(projectReserved);
        BuildReservedFolder(ProjectItemType.MasterPage, foundItems, projectReserved, "Master Pages");
        BuildReservedFolder(ProjectItemType.StyleSheet, foundItems, projectReserved, "Styles");
        BuildReservedFolder(ProjectItemType.Script, foundItems, projectReserved, "Scripts");

        foreach (ProjectItem item in foundItems.Where(x => !x.Name.Equals(RESERVED_FOLDER_NAME)))
        {
            rootItem.Children.Add(item);
        }
    }

    private void BuildReferencesFolder(ProjectItem rootItem)
    {
        ProjectItem packagesItem = new()
        {
            Name = "References",
            ItemType = ProjectItemType.WebReference
        };

        foreach (PackageReference reference in PackageReferences)
        {
            packagesItem.Children.Add(new ProjectItem
            {
                Name = $"{reference.Name} - {reference.Version}",
                ItemType = ProjectItemType.WebReference
            });
        }

        rootItem.Children.Add(packagesItem);
    }
}