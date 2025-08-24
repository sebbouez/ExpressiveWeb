// *********************************************************
// 
// ExpressiveWeb EditorView.axaml.cs
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
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using ExpressiveWeb.Commands;
using ExpressiveWeb.Core;
using ExpressiveWeb.Core.ApplicationCommands;
using ExpressiveWeb.Core.Commands;
using ExpressiveWeb.Core.FileManagement;
using ExpressiveWeb.Core.Kit.ComponentFeatures;
using ExpressiveWeb.Designer;
using ExpressiveWeb.Designer.Models;
using ExpressiveWeb.Presentation.Menus;
using ExpressiveWeb.Workspace;
using Microsoft.Extensions.DependencyInjection;

namespace ExpressiveWeb.Modules.EditorView;

public partial class EditorView : UserControl, IWorkspaceTabViewBase
{
    private readonly IApplicationCommandsService _applicationCommandsService;
    private readonly BusinessCommandManager _commandManager = new();
    private bool _isLoaded;

    public EditorView()
    {
        InitializeComponent();
        Background = Brushes.Transparent;
        Loaded += OnLoaded;

        _applicationCommandsService = AppServices.ServicesFactory!.GetService<IApplicationCommandsService>()!;
    }

    internal HtmlEditor Editor
    {
        get;
        private set;
    }

    public required string FilePath
    {
        get;
        init;
    }

    public void Activated()
    {
        UpdateCommands();
    }

    public void Deactivated()
    {
    }

    public void Closed()
    {
        Editor.Dispose();
    }

    public bool CanClose()
    {
        return !_commandManager.HasCommands;
    }

    public event EventHandler<bool>? IsDirtyStateChanged;

    public async Task SaveAsync()
    {
        string htmlContent = await Editor.GetFilteredHtml();

        AppState.Instance.AppWindow.SetStatusMessage(Localization.Resources.StatusSaving);

        using VariableDurationActionHandler hanlder = new(250);
        try
        {
            await hanlder.Run(() =>
            {
                FilesAccessHelper.WriteAllText(FilePath, htmlContent);
            });

            AppState.Instance.AppWindow.SetStatusMessage(Localization.Resources.StatusSaved);
        }
        catch (Exception ex)
        {
            AppState.Instance.AppWindow.SetStatusMessage("Error saving.");
            Console.WriteLine(ex.Message);
        }
    }

    private void BuildContextMenu()
    {
        FilterableContextMenu menu = new();
        menu.AppendCommand<CutCommand>();
        menu.AppendCommand<CopyCommand>();
        menu.AppendCommand<PasteCommand>();
        menu.AppendCommand<SeparatorCommand>();
        menu.AppendCommand<DeleteElementCommand>();
        menu.AppendCommand<DuplicateElementCommand>();
        Editor.ContextMenu = menu;
    }

    private void CommandManagerOnChanged(object? sender, bool e)
    {
        UpdateCommands();
        IsDirtyStateChanged?.Invoke(this, _commandManager.HasCommands);
    }

    private void EditorOnSelectionChanged(object? sender, object? e)
    {
        if (e is HtmlElement element)
        {
            AppState.Instance.AppWindow.SetStatusMessage(element.KitComponent?.Name ?? element.TagName);
            _applicationCommandsService.SetCommandState<DuplicateElementCommand>(element.KitComponent != null);
            _applicationCommandsService.SetCommandState<DeleteElementCommand>(element.KitComponent != null);
        }
        else
        {
            AppState.Instance.AppWindow.SetStatusMessage(Localization.Resources.StatusSelectMessage);
            _applicationCommandsService.SetCommandState<DuplicateElementCommand>(false);
            _applicationCommandsService.SetCommandState<DeleteElementCommand>(false);
        }

        UpdateTextTagsCommands();
    }

    private void EdOnInfoRaised(object? sender, string e)
    {
        AppState.Instance.AppWindow.SetStatusMessage(e);
    }

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        if (_isLoaded)
        {
            return;
        }

        _isLoaded = true;


        _commandManager.Changed += CommandManagerOnChanged;

        Editor = new HtmlEditor(AppState.Instance.CurrentProject!.Kit);
        Editor.CommandManager = _commandManager;
        Editor.SelectionChanged += EditorOnSelectionChanged;
        Editor.InfoRaised += EdOnInfoRaised;
        Border20.Child = Editor;


        BuildContextMenu();


        Editor.LoadPage(FilePath);
    }

    [Obsolete("Do not use this method. It is only for debugging purpose.")]
    internal void OpenDevTools()
    {
        Editor.ShowDevTools();
    }

    public void Redo()
    {
        if (_commandManager.CanRedo)
        {
            _commandManager.Redo();
        }
    }

    public void Undo()
    {
        if (_commandManager.CanUndo)
        {
            _commandManager.Undo();
        }
    }

    private void UpdateCommands()
    {
        _applicationCommandsService.SetCommandState<SaveCommand>(true);
        _applicationCommandsService.SetCommandState<RedoCommand>(_commandManager.CanRedo);
        _applicationCommandsService.SetCommandState<UndoCommand>(_commandManager.CanUndo);

        UpdateTextTagsCommands();
    }

    private void UpdateTextTagsCommands()
    {
        bool canChangeTextTag = Editor != null && Editor.SelectedElement is HtmlElement element && element.KitComponent != null && element.KitComponent.HasFeature<SwapTextTagFeature>();

        _applicationCommandsService.SetCommandState<FormatHeading1Command>(canChangeTextTag);
        _applicationCommandsService.SetCommandState<FormatHeading2Command>(canChangeTextTag);
        _applicationCommandsService.SetCommandState<FormatHeading3Command>(canChangeTextTag);
        _applicationCommandsService.SetCommandState<FormatHeading4Command>(canChangeTextTag);
        _applicationCommandsService.SetCommandState<FormatHeading5Command>(canChangeTextTag);
        _applicationCommandsService.SetCommandState<FormatHeading6Command>(canChangeTextTag);
        _applicationCommandsService.SetCommandState<FormatParagraphCommand>(canChangeTextTag);
    }
}