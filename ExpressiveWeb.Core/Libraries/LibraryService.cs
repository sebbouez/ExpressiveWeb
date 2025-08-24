// *********************************************************
// 
// ExpressiveWeb.Core LibraryService.cs
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

using System.Xml.Linq;
using ExpressiveWeb.Core.Env;
using ExpressiveWeb.Core.Log;

namespace ExpressiveWeb.Core.Libraries;

public class LibraryService : ILibraryService
{
    private static bool TryGetCategoryFromKey(string key, out CategoryNodeType category)
    {
        category = CategoryNodeType.None;
        if (string.IsNullOrWhiteSpace(key))
        {
            return false;
        }

        switch (key.Trim())
        {
            case var s when s.Equals("style", StringComparison.OrdinalIgnoreCase):
                category = CategoryNodeType.Style;
                return true;
            case var s when s.Equals("palette", StringComparison.OrdinalIgnoreCase):
                category = CategoryNodeType.Palette;
                return true;
            case var s when s.Equals("image", StringComparison.OrdinalIgnoreCase):
                category = CategoryNodeType.Image;
                return true;
            default:
                return false;
        }
    }
    private static readonly IReadOnlyDictionary<CategoryNodeType, Type> ItemsTypeMappingCache = new Dictionary<CategoryNodeType, Type>
    {
        [CategoryNodeType.Style] = typeof(LibraryStyleItem),
        [CategoryNodeType.Palette] = typeof(LibraryPaletteItem),
        [CategoryNodeType.Image] = typeof(LibraryImageItem)
    };

    private readonly IEnvironmentService _environmentService;
    private readonly ILogService _logService;

    public LibraryService(ILogService logService, IEnvironmentService environmentService)
    {
        _environmentService = environmentService;
        _logService = logService;
    }

    public List<LibraryItemBase> LibraryItems
    {
        get;
        set;
    } = [];

    public Type? GetItemTypeForCategory(CategoryNodeType category)
    {
        return ItemsTypeMappingCache.GetValueOrDefault(category);
    }

    public IEnumerable<LibraryItemBase> GetItemsByCategory(CategoryNodeType category)
    {
        Type? type = GetItemTypeForCategory(category);
        if (type == null)
        {
            return [];
        }

        return LibraryItems.Where(item => type.IsInstanceOfType(item));
    }

    public async Task LoadItemsAsync(CancellationToken cancellationToken = default)
    {
        string libraryFolderPath = _environmentService.LibraryFolderPath;
        LibraryItems.Clear();

        try
        {
            if (!Directory.Exists(libraryFolderPath))
            {
                _logService.Warning($"Library folder not found: {libraryFolderPath}");
                return;
            }

            foreach (string file in Directory.EnumerateFiles(libraryFolderPath, "*.xml", SearchOption.TopDirectoryOnly))
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                try
                {
                    await using FileStream fs = File.OpenRead(file);
                    XDocument doc = await XDocument.LoadAsync(fs, LoadOptions.None, cancellationToken);
                    XElement? root = doc.Root;
                    if (root == null)
                    {
                        _logService.Warning($"Skipping library file with no root element: {file}");
                        continue;
                    }

                    string? typeKey = root.Element("Type")?.Value?.Trim();
                    if (string.IsNullOrEmpty(typeKey))
                    {
                        _logService.Warning($"Skipping library file with missing <Type>: {file}");
                        continue;
                    }

                    if (!TryGetCategoryFromKey(typeKey, out CategoryNodeType category))
                    {
                        _logService.Warning($"Unknown library item type '{typeKey}' in file {file}; skipping.");
                        continue;
                    }

                    Type? itemType = GetItemTypeForCategory(category);
                    if (itemType == null)
                    {
                        _logService.Warning($"No mapped .NET Type for category '{category}' in file {file}; skipping.");
                        continue;
                    }

                    if (Activator.CreateInstance(itemType) is not LibraryItemBase item)
                    {
                        _logService.Warning($"Failed to instantiate library item of type '{itemType.FullName}' from file {file}; skipping.");
                        continue;
                    }

                    item.Load(doc);
                    LibraryItems.Add(item);
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
    }
}