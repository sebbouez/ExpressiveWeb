// *********************************************************
// 
// ExpressiveWeb OnlineStatusPanel.axaml.cs
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
using ExpressiveWeb.Core.Network;
using Microsoft.Extensions.DependencyInjection;

namespace ExpressiveWeb.Panels;

public partial class OnlineStatusPanel : UserControl
{
    public OnlineStatusPanel()
    {
        InitializeComponent();
    }

    private void BtnSwitch_OnClick(object? sender, RoutedEventArgs e)
    {
        AppServices.ServicesFactory!.GetService<INetworkService>()!.Mode = BtnSwitch.IsChecked!.Value ? NetworkServiceMode.RequiredOffline : NetworkServiceMode.Auto;
    }
}