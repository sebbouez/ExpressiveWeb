// *********************************************************
// 
// ExpressiveWeb PackagesManagerDialog.axaml.cs
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
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using PackagesProviders;

namespace ExpressiveWeb.Modules.PackagesManager;

public partial class PackagesManagerDialog : Window
{
    public PackagesManagerDialog()
    {
        InitializeComponent();
    }

    private void BtnCancel_OnClick(object? sender, RoutedEventArgs e)
    {
        Close(false);
    }

    private void BtnOk_OnClick(object? sender, RoutedEventArgs e)
    {
        Close(true);
    }

    private void TbSearch_OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            JsDeliverPackageProvider provider = new();

            provider.GetPackages(this.TbSearch.Text).ContinueWith(task =>
            {
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
    }
}