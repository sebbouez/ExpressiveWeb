// *********************************************************
// 
// ExpressiveWeb.UI.Internal BackgroundTasksManagerIndicator.cs
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
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Media;
using ExpressiveWeb.Core.BackgroundServices;

namespace ExpressiveWeb.Presentation.StatusBarPopupButton;

public class StatusBarPopupButton : TemplatedControl
{
    ProgressBar _progressBar;
    TextBlock _textBlock;
    ToggleButton _toggleButton;
    Flyout? _flyout;

    private IBackgroundTaskManager? _taskManager;

    public static readonly StyledProperty<object?> PopupContentProperty =
        AvaloniaProperty.Register<StatusBarPopupButton, object?>(nameof(PopupContent), null);

    public static readonly StyledProperty<string> TextProperty =
        AvaloniaProperty.Register<StatusBarPopupButton, string>(nameof(Text), string.Empty);

    public static readonly StyledProperty<IBrush?> IconBrushProperty =
        AvaloniaProperty.Register<StatusBarPopupButton, IBrush?>(nameof(IconBrush), Brushes.Transparent);

    public IBrush? IconBrush
    {
        get
        {
            return GetValue(IconBrushProperty);
        }
        set
        {
            SetValue(IconBrushProperty, value);
        }
    }

    public string Text
    {
        get
        {
            return GetValue(TextProperty);
        }
        set
        {
            SetValue(TextProperty, value);
        }
    }

    public object PopupContent
    {
        get
        {
            return GetValue(PopupContentProperty);
        }
        set
        {
            SetValue(PopupContentProperty, value);
        }
    }

    public void BindService(IBackgroundTaskManager service)
    {
        _taskManager = service;
        _taskManager.StatusChanged += TaskManagerOnStatusChanged;
    }

    private void TaskManagerOnStatusChanged(object? sender, BackgroundTaskManagerStatus e)
    {
        _toggleButton.IsVisible = e != BackgroundTaskManagerStatus.Idle;
    }

    public StatusBarPopupButton()
    {
        Loaded += OnLoaded;
    }

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        _toggleButton = e.NameScope.Find<ToggleButton>("PART_Btn");
        _textBlock = e.NameScope.Find<TextBlock>("PART_Text");

        if (_toggleButton != null)
        {
            _flyout = (FlyoutBase.GetAttachedFlyout(_toggleButton) as Flyout);
            _flyout.Closed -= OnClosed;
            _flyout.Closed += OnClosed;
            _flyout.Placement = PlacementMode.Custom;
            _flyout.CustomPopupPlacementCallback = (p) =>
            {
                p.Offset = new Point(-p.PopupSize.Width / 2 + p.AnchorRectangle.Width / 2, -p.PopupSize.Height / 2 - p.AnchorRectangle.Height / 2);
            };

            _toggleButton.Click += ToggleButtonOnClick;
        }
    }

    private void ToggleButtonOnClick(object? sender, RoutedEventArgs e)
    {
        if (_toggleButton.IsChecked!.Value)
        {
            _flyout!.ShowAt(_toggleButton);
        }
        else
        {
            _flyout!.Hide();
        }
    }

    private void OnClosed(object? sender, EventArgs e)
    {
        _toggleButton.IsChecked = false;
    }
}