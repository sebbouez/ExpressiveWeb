// *********************************************************
// 
// ExpressiveWeb StylePanel.axaml.cs
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

using Avalonia.Controls;
using Avalonia.Interactivity;
using ExpressiveWeb.Core;
using ExpressiveWeb.Core.Style;
using Microsoft.Extensions.DependencyInjection;

namespace ExpressiveWeb.Panels.Styles;

public partial class StylePanel : UserControl
{
    private readonly IStyleService _styleService;

    public StylePanel()
    {
        InitializeComponent();
        _styleService = AppServices.ServicesFactory.GetService<IStyleService>();
        
        Loaded+= OnLoaded;
    }

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {

    //    CbStylesList.ItemsSource = AppState.Instance.CurrentProject!.UserStyles;

    }
}