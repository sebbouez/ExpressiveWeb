// *********************************************************
// 
// ExpressiveWeb CreatePageDialog.axaml.cs
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

using System.IO;
using System.Runtime.InteropServices.JavaScript;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using ExpressiveWeb.Core;
using ExpressiveWeb.Core.Kit;
using Microsoft.Extensions.DependencyInjection;

namespace ExpressiveWeb.Modules.NewPage;

public partial class CreatePageDialog : Window
{
    public CreatePageDialog()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        ComboTemplates.ItemsSource = AppState.Instance.CurrentProject!.Kit.Templates;
        ComboTemplates.DisplayMemberBinding = new Binding(nameof(KitPageTemplate.Name));
        ComboTemplates.SelectedIndex = 0;
    }

    internal string FilePath
    {
        get;
       private set;
    }

    internal KitPageTemplate SelectedTemplate
    {
        get;
        private set;
    }

    private void BtnOk_OnClick(object? sender, RoutedEventArgs e)
    {
        FilePath = Path.Combine(AppState.Instance.CurrentProject.RootPath, TbFileName.Text);
        SelectedTemplate = (KitPageTemplate) ComboTemplates.SelectedItem;

        

        this.Close(true);
    }

    private void BtnCancel_OnClick(object? sender, RoutedEventArgs e)
    {
        this.Close(false);
    }

    private void TbFileName_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        // TODO : do better checks for valid file name
        BtnOk.IsEnabled = !string.IsNullOrEmpty(TbFileName.Text);
    }
}