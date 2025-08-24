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
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using ExpressiveWeb.Commands;
using ExpressiveWeb.Core;
using ExpressiveWeb.Core.ApplicationCommands;
using ExpressiveWeb.Core.BackgroundServices;
using ExpressiveWeb.Core.Kit;
using ExpressiveWeb.Core.Network;
using ExpressiveWeb.Core.Project;
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
    private HtmlEditor _ed;

    private ExplorerControl _explorerControl;
    private IApplicationCommandsService _applicationCommandsService;

    public MainWindow()
    {
        InitializeComponent();
        Loaded += OnLoaded;
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
        // MainMenuBarControl.AppendMenu(Localization.Resources.MenuView, GetFileCommands());
        MainMenuBarControl.AppendMenu(Localization.Resources.MenuCommunity, GetCommunityCommands());
        MainMenuBarControl.AppendMenu(Localization.Resources.MenuHelp, GetHelpCommands());
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
            new DuplicateElementCommand(),
            new DevToolsCommand()
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
            new FormatHeading6Command(),
        };

        return result;
    }

    private List<ApplicationCommandBase> GetHelpCommands()
    {
        List<ApplicationCommandBase> result = new()
        {
            new AboutApplicationCommand(),
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

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        INetworkService networkService = AppServices.ServicesFactory!.GetService<INetworkService>()!;
        AppServices.ServicesFactory!.GetService<IBackgroundTaskManager>()!.StatusChanged += OnStatusChanged;
        TasksManagerIndicator.BindService(AppServices.ServicesFactory!.GetService<IBackgroundTaskManager>()!);


        _applicationCommandsService = AppServices.ServicesFactory!.GetService<IApplicationCommandsService>()!;

        networkService.ModeChanged += NetworkServiceOnModeChanged;

        ApplicationSharedEvents.ProjectLoaded += ApplicationSharedEventsOnProjectLoaded;

        NetworkServiceOnModeChanged(networkService, EventArgs.Empty);
        BuildMenuBar();

        InitToolsPanel();
    }

    private void OnStatusChanged(object? sender, BackgroundTaskManagerStatus e)
    {
    }

    public void SetStatusMessage(string message)
    {
        Dispatcher.UIThread.Post(() =>
        {
            TbStatus.Text = message;
        });
    }
}