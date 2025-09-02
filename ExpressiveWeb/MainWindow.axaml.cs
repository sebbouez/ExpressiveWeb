// *********************************************************
// 
// ExpressiveWeb MainWindow.axaml.cs
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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;
using ExpressiveWeb.Commands;
using ExpressiveWeb.Core;
using ExpressiveWeb.Core.ApplicationCommands;
using ExpressiveWeb.Core.BackgroundServices;
using ExpressiveWeb.Core.Network;
using ExpressiveWeb.Core.Project;
using ExpressiveWeb.Core.Settings;
using ExpressiveWeb.Designer;
using ExpressiveWeb.Modules.EditorView;
using ExpressiveWeb.Modules.Explorer;
using ExpressiveWeb.Panels.Collections;
using ExpressiveWeb.Panels.Libraries;
using ExpressiveWeb.Panels.Pages;
using ExpressiveWeb.Panels.Styles;
using ExpressiveWeb.Workspace;
using Microsoft.Extensions.DependencyInjection;
using SeparatorCommand = ExpressiveWeb.Core.ApplicationCommands.SeparatorCommand;

namespace ExpressiveWeb;

public partial class MainWindow : Window
{
    private readonly IApplicationCommandsService _applicationCommandsService;
    private HtmlEditor _ed;

    private ExplorerControl _explorerControl;
    private readonly ISettingsService _settingsService;

    public MainWindow()
    {
        InitializeComponent();

        _settingsService = AppServices.ServicesFactory!.GetService<ISettingsService>()!;
        _applicationCommandsService = AppServices.ServicesFactory!.GetService<IApplicationCommandsService>()!;

        Loaded += OnLoaded;
        Closing += OnClosing;
    }

    private void ApplicationSharedEventsOnProjectLoaded(object? sender, Project e)
    {
        _explorerControl.Items = e.Items;

        _applicationCommandsService.SetCommandState<NewFolderCommand>(true);
        _applicationCommandsService.SetCommandState<NewPageCommand>(true);
    }

    private void BuildMenuBar()
    {
        MainMenuBarControl.AppendMenu(Localization.Resources.MenuFile, GetFileCommands());
        MainMenuBarControl.AppendMenu(Localization.Resources.MenuEdit, GetEditCommands());
        // MainMenuBarControl.AppendMenu(Localization.Resources.MenuInsert, GetFileCommands());
        MainMenuBarControl.AppendMenu(Localization.Resources.MenuFormat, GetFormatCommands());
        MainMenuBarControl.AppendMenu(Localization.Resources.MenuCommunity, GetCommunityCommands());
        MainMenuBarControl.AppendMenu(Localization.Resources.MenuView, GetViewCommands());
        MainMenuBarControl.AppendMenu(Localization.Resources.MenuHelp, GetHelpCommands());
    }

    private void BuildToolBar()
    {
        ISettingsService settingsService = AppServices.ServicesFactory!.GetService<ISettingsService>()!;

        List<ApplicationCommandBase?> leftCommands = settingsService.UserSettings.UISettings.MainToolbarLeftCommands.Select(commandName => _applicationCommandsService.GetCommand(commandName)).ToList();
        MainToolbarControl.SetLeftToolbarItems(leftCommands);

        List<ApplicationCommandBase?> centerCommands = settingsService.UserSettings.UISettings.MainToolbarCenterCommands.Select(commandName => _applicationCommandsService.GetCommand(commandName)).ToList();
        MainToolbarControl.SetCenterToolbarItems(centerCommands);

        List<ApplicationCommandBase?> rightCommands = settingsService.UserSettings.UISettings.MainToolbarRightCommands.Select(commandName => _applicationCommandsService.GetCommand(commandName)).ToList();
        MainToolbarControl.SetRightToolbarItems(rightCommands);
    }

    private void Explorer_OnItemDoubleClicked(object? sender, ProjectItem e)
    {
        if (e.ItemType == ProjectItemType.File && !string.IsNullOrEmpty(e.Path))
        {
            EditorView editorWorkspace = new()
            {
                FilePath = e.Path
            };

            ApplicationWorkspaceControl.OpenDocumentTab(e.Path, new FileInfo(e.Path).Name, editorWorkspace);
        }
    }

    private void FormatStatusText(TextBlock textBlock, string text)
    {
        textBlock.Inlines?.Clear();

        Regex regex = new(@"\*\*(.*?)\*\*");
        MatchCollection matches = regex.Matches(text);

        int lastIndex = 0;

        foreach (Match match in matches)
        {
            if (match.Index > lastIndex)
            {
                string normalText = text.Substring(lastIndex, match.Index - lastIndex);
                if (!string.IsNullOrEmpty(normalText))
                {
                    textBlock.Inlines?.Add(new Run(normalText));
                }
            }

            string boldText = match.Groups[1].Value;
            if (!string.IsNullOrEmpty(boldText))
            {
                Run boldRun = new(boldText)
                {
                    FontWeight = FontWeight.SemiBold
                };
                textBlock.Inlines?.Add(boldRun);
            }

            lastIndex = match.Index + match.Length;
        }

        if (lastIndex < text.Length)
        {
            string remainingText = text.Substring(lastIndex);
            if (!string.IsNullOrEmpty(remainingText))
            {
                textBlock.Inlines?.Add(new Run(remainingText));
            }
        }
    }

    private List<ApplicationCommandBase> GetCommunityCommands()
    {
        List<ApplicationCommandBase> result = new()
        {
            new CommunityAskQuestionCommand(),
            new CommunityDevCenterCommand(),
            new SeparatorCommand(),
            new CommunitySendFeedbackCommand()
        };

        return result;
    }

    private List<ApplicationCommandBase> GetEditCommands()
    {
        List<ApplicationCommandBase> result = new()
        {
            new UndoCommand(),
            new RedoCommand(),
            new SeparatorCommand(),
            new CutCommand(),
            new CopyCommand(),
            new PasteCommand(),
            new SeparatorCommand(),
            new DeleteElementCommand(),
            new DuplicateElementCommand(),

#if DEBUG
            new SeparatorCommand(),
            new DevToolsCommand(),
#endif
            new SeparatorCommand(),
            new SelectParentElementCommand(),
            new SeparatorCommand(),
            new MoveElementFirstCommand(),
            new MoveElementBeforeCommand(),
            new MoveElementAfterCommand(),
            new MoveElementLastCommand()
        };

        return result;
    }

    private List<ApplicationCommandBase> GetFileCommands()
    {
        List<ApplicationCommandBase> result = new()
        {
            new NewCommand(),
            new OpenCommand(),
            new SeparatorCommand(),
            new SaveCommand(),
            new SeparatorCommand(),
            new ExitApplicationCommand()
        };

        return result;
    }

    private List<ApplicationCommandBase> GetFormatCommands()
    {
        List<ApplicationCommandBase> result = new()
        {
            new FormatParagraphCommand(),
            new FormatHeading1Command(),
            new FormatHeading2Command(),
            new FormatHeading3Command(),
            new FormatHeading4Command(),
            new FormatHeading5Command(),
            new FormatHeading6Command()
        };

        return result;
    }

    private List<ApplicationCommandBase> GetHelpCommands()
    {
        List<ApplicationCommandBase> result = new()
        {
            new AboutApplicationCommand()
        };

        return result;
    }

    private List<ApplicationCommandBase> GetViewCommands()
    {
        List<ApplicationCommandBase> result = new()
        {
            new CloseCurrentDocumentCommand(),
            new CloseOtherDocumentsCommand(),
            new CloseAllDocumentsCommand(),
            new SeparatorCommand(),
            new CloseDocumentsLeftCommand(),
            new CloseDocumentsRightCommand()
        };

        return result;
    }

    private void InitToolsPanel()
    {
        // TODO panels should be dynamically created with configuration

        _explorerControl = new ExplorerControl();
        _explorerControl.ItemDoubleClicked += Explorer_OnItemDoubleClicked;

        ApplicationWorkspaceControl.AddPanel(_explorerControl, "Explorer", ApplicationWorkspace.PanelPosition.Left);

        StylePanel cc = new();
        ApplicationWorkspaceControl.AddPanel(cc, "Style", ApplicationWorkspace.PanelPosition.Right);

        PagePropertiesPanel panel = new();
        ApplicationWorkspaceControl.AddPanel(panel, "Page", ApplicationWorkspace.PanelPosition.Right);

        LibraryPanel libraryPanel = new();
        ApplicationWorkspaceControl.AddPanel(libraryPanel, "Library", ApplicationWorkspace.PanelPosition.Right);

        PalettesPanel palettesPanel = new();
        ApplicationWorkspaceControl.AddPanel(palettesPanel, "Palettes", ApplicationWorkspace.PanelPosition.Right);
    }

    private void NetworkServiceOnModeChanged(object? sender, EventArgs e)
    {
        if (((INetworkService) sender!).IsConnected())
        {
            BtnStatusOnline.Text = Localization.Resources.IsConnected;
            BtnStatusOnline.IconBrush = (Brush?) this.FindResource("IconOnline");
        }
        else
        {
            BtnStatusOnline.Text = Localization.Resources.IsDisconnected;
            BtnStatusOnline.IconBrush = (Brush?) this.FindResource("IconOffline");
        }
    }

    private void OnClosing(object? sender, WindowClosingEventArgs e)
    {
        _settingsService.UserSettings.UISettings.MainWindowSize.WindowWidth = (int) Width;
        _settingsService.UserSettings.UISettings.MainWindowSize.WindowHeight = (int) Height;
        _settingsService.UserSettings.UISettings.MainWindowSize.WindowX = Position.X;
        _settingsService.UserSettings.UISettings.MainWindowSize.WindowY = Position.Y;
        _settingsService.UserSettings.UISettings.MainWindowSize.IsMaximized = WindowState == WindowState.Maximized;

        _settingsService.SaveSettings();
    }

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        INetworkService networkService = AppServices.ServicesFactory!.GetService<INetworkService>()!;
        AppServices.ServicesFactory!.GetService<IBackgroundTaskManager>()!.StatusChanged += OnStatusChanged;
        TasksManagerIndicator.BindService(AppServices.ServicesFactory!.GetService<IBackgroundTaskManager>()!);


        networkService.ModeChanged += NetworkServiceOnModeChanged;
        ApplicationSharedEvents.ProjectLoaded += ApplicationSharedEventsOnProjectLoaded;

        NetworkServiceOnModeChanged(networkService, EventArgs.Empty);


        RestoreWindowSize();

        BuildMenuBar();
        BuildToolBar();

        InitToolsPanel();
    }

    private void OnStatusChanged(object? sender, BackgroundTaskManagerStatus e)
    {
    }

    private void RestoreWindowSize()
    {
        if (_settingsService.UserSettings.UISettings.MainWindowSize.WindowWidth != null
            && _settingsService.UserSettings.UISettings.MainWindowSize.WindowHeight != null
            && _settingsService.UserSettings.UISettings.MainWindowSize.WindowWidth > 100
            && _settingsService.UserSettings.UISettings.MainWindowSize.WindowHeight > 100
            && !_settingsService.UserSettings.UISettings.MainWindowSize.IsMaximized)
        {
            Width = _settingsService.UserSettings.UISettings.MainWindowSize.WindowWidth!.Value;
            Height = _settingsService.UserSettings.UISettings.MainWindowSize.WindowHeight!.Value;
        }

        if (_settingsService.UserSettings.UISettings.MainWindowSize.WindowX != null
            && _settingsService.UserSettings.UISettings.MainWindowSize.WindowY != null
            && _settingsService.UserSettings.UISettings.MainWindowSize.WindowX > 0
            && _settingsService.UserSettings.UISettings.MainWindowSize.WindowY > 0
            && !_settingsService.UserSettings.UISettings.MainWindowSize.IsMaximized)
        {
            int x = _settingsService.UserSettings.UISettings.MainWindowSize.WindowX!.Value;
            int y = _settingsService.UserSettings.UISettings.MainWindowSize.WindowY!.Value;
            x = x < 0 ? 0 : x;
            y = y < 0 ? 0 : y;
            Position = new PixelPoint(x, y);
        }
    }

    public void SetStatusMessage(string message)
    {
        Dispatcher.UIThread.Post(() =>
        {
            FormatStatusText(TbStatus, message);
        });
    }
}