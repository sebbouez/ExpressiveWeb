// *********************************************************
// 
// ExpressiveWeb LibraryPanel.axaml.cs
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

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using ExpressiveWeb.Core;
using ExpressiveWeb.Core.Libraries;
using Microsoft.Extensions.DependencyInjection;

namespace ExpressiveWeb.Panels.Libraries;

public partial class LibraryPanel : UserControl
{
    // Collections pour les données affichées
    private readonly ObservableCollection<CategoryNodeModel> _categories = new();
    private readonly ObservableCollection<LibraryItemBase> _displayedItems = new();
    private readonly ILibraryService _libraryService;

    private bool _isLoaded;

    public LibraryPanel()
    {
        InitializeComponent();
        _libraryService = AppServices.ServicesFactory!.GetService<ILibraryService>()!;

        // Liaison des collections aux contrôles
        TvCategories.ItemsSource = _categories;
        LstItems.ItemsSource = _displayedItems;

        // Gestion de la sélection dans le TreeView
        TvCategories.SelectionChanged += OnCategorySelectionChanged;

        Loaded += OnLoaded;
    }

    private void LoadItemsForCategory(CategoryNodeType categoryType)
    {
        _displayedItems.Clear();

        if (categoryType == CategoryNodeType.None)
        {
            return;
        }

        IEnumerable<LibraryItemBase> itemsToShow = _libraryService.GetItemsByCategory(categoryType);

        foreach (LibraryItemBase item in itemsToShow)
        {
            _displayedItems.Add(item);
        }
    }

    private void OnCategorySelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        CategoryNodeModel? selectedCategory = null;
        if (e.AddedItems.Count > 0)
        {
            selectedCategory = e.AddedItems[0] as CategoryNodeModel;
        }

        if (selectedCategory is null)
        {
            _displayedItems.Clear();
            return;
        }

        LoadItemsForCategory(selectedCategory.Type);
    }

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        if (_isLoaded)
        {
            return;
        }

        _isLoaded = true;

        _libraryService.LoadItemsAsync().ContinueWith(task =>
        {
            if (task.IsCompletedSuccessfully)
            {
                PopulateCategories();
            }
        }, TaskScheduler.FromCurrentSynchronizationContext());
    }

    private void PopulateCategories()
    {
        _categories.Clear();

        int styleCount = _libraryService.LibraryItems.OfType<LibraryStyleItem>().Count();
        int paletteCount = _libraryService.LibraryItems.OfType<LibraryPaletteItem>().Count();
        int imageCount = _libraryService.LibraryItems.OfType<LibraryImageItem>().Count();

        CategoryNodeModel libRootNode = new(Localization.Resources.Library, CategoryNodeType.None, _libraryService.LibraryItems.Count);
        _categories.Add(libRootNode);

        libRootNode.Children.Add(new CategoryNodeModel(Localization.Resources.LibraryStyles, CategoryNodeType.Style, styleCount));
        libRootNode.Children.Add(new CategoryNodeModel(Localization.Resources.LibraryPalettes, CategoryNodeType.Palette, paletteCount));
        libRootNode.Children.Add(new CategoryNodeModel(Localization.Resources.LibraryImages, CategoryNodeType.Image, imageCount));
    }
}