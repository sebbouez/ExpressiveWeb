// *********************************************************
// 
// ExpressiveWeb GalleryPanel.axaml.cs
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

using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using ExpressiveWeb.Core.Project;

namespace ExpressiveWeb.Panels.Gallery;

public partial class GalleryPanel : UserControl
{
    public GalleryPanel()
    {
        InitializeComponent();
        Loaded += OnLoaded;
        Unloaded+= OnUnloaded;
    }

    private void OnUnloaded(object? sender, RoutedEventArgs e)
    {
        ApplicationSharedEvents.ProjectLoaded-= ApplicationSharedEventsOnProjectLoaded;
    }

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        if (AppState.Instance.CurrentProject == null)
        {
            return;
        }

        ApplicationSharedEvents.ProjectLoaded+= ApplicationSharedEventsOnProjectLoaded;
        ApplicationSharedEventsOnProjectLoaded(this, null);
    }

    private void ApplicationSharedEventsOnProjectLoaded(object? sender, Project? e)
    {
        LsGalleryItems.ItemsSource = AppState.Instance.CurrentProject!.Kit.GalleryItems;
    }
}