// *********************************************************
// 
// ExpressiveWeb CreateProjectDialog.axaml.cs
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

using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using ExpressiveWeb.CommonDialogs;
using ExpressiveWeb.Core;
using ExpressiveWeb.Core.FileManagement;
using ExpressiveWeb.Core.Kit;
using ExpressiveWeb.Core.Project;
using ExpressiveWeb.Presentation.MessageBox;
using Microsoft.Extensions.DependencyInjection;

namespace ExpressiveWeb.Modules.NewProject;

public partial class CreateProjectDialog : Window
{
    private readonly IKitService _kitService;

    public CreateProjectDialog()
    {
        InitializeComponent();
        _kitService = AppServices.ServicesFactory!.GetService<IKitService>()!;
        Loaded += OnLoaded;
    }

    private async void BtnBrowse_OnClick(object? sender, RoutedEventArgs e)
    {
        FilePickerSaveOptions options = new()
        {
            Title = "...",
            FileTypeChoices = new[]
            {
                new FilePickerFileType("Expressive Web Project")
                {
                    Patterns = new[] {"*.ewproj"}
                }
            },
            DefaultExtension = "ewproj",
            ShowOverwritePrompt = true
        };

        IStorageFile? r = await StorageProvider.SaveFilePickerAsync(options);

        if (r != null)
        {
            TbFilePath.Text = r.Path.LocalPath;
        }
    }

    private void BtnCancel_OnClick(object? sender, RoutedEventArgs e)
    {
        Close(string.Empty);
    }

    private void BtnOk_OnClick(object? sender, RoutedEventArgs e)
    {
        if (ComboKits.SelectedItem is not Kit selectedKit)
        {
            return;
        }

        string targetFilePath = TbFilePath.Text!;

        using VariableDurationActionHandler handler = new VariableDurationActionHandler(350);
        handler.Run(async void () =>
        {
            ProjectLoader loader = new();
            await loader.Create(selectedKit, targetFilePath);
        }).ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                EWMessageBox.Show(new MessageBoxData
                {
                    Owner = this,
                    Buttons = MessageBoxButtons.Ok,
                    Message = "Error creating project",
                    Title = "Error"
                });
            }

            Close(targetFilePath);
        }, TaskScheduler.FromCurrentSynchronizationContext());
    }

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        _kitService.LoadKits().ContinueWith(task =>
        {
            ComboKits.ItemsSource = task.Result;
            ComboKits.DisplayMemberBinding = new Binding(nameof(Kit.Name));
            ComboKits.SelectedIndex = 0;
        }, TaskScheduler.FromCurrentSynchronizationContext());
    }

    private void TbFilePath_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        BtnOk.IsEnabled = FilesAccessHelper.CheckFileIsValid(TbFilePath.Text ?? "");
    }
}