// *********************************************************
// 
// ExpressiveWeb ApplicationWorkspace.cs
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
using System.Linq;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using Dock.Avalonia.Controls;
using Dock.Model.Avalonia;
using Dock.Model.Avalonia.Controls;
using Dock.Model.Controls;
using Dock.Model.Core;
using Dock.Model.Core.Events;

namespace ExpressiveWeb.Workspace;

public class ApplicationWorkspace : ContentControl
{
    public enum PanelPosition
    {
        Left,
        Right
    }

    private readonly DocumentDock _documentDock;
    private readonly Factory _factory;

    // Références aux ToolDocks principaux
    private readonly ToolDock? _leftToolDock;
    private readonly ToolDock? _rightToolDock;
    private readonly DockControl _dockControl;

    private WorkspaceView? _lastView;

    public ApplicationWorkspace()
    {
        _dockControl = new DockControl();

        // Utilisation des instances passées en argument, si fourni
        _factory = new Factory
        {
            DefaultHostWindowLocator = () => new HostWindow
            {
                Background = new SolidColorBrush(Color.FromArgb(0xFF, 0x1E, 0x1E, 0x1E))
            }
        };

        _factory.FocusedDockableChanged += FactoryOnFocusedDockableChanged;
        _factory.DockableClosing += FactoryOnDockableClosing;

        _documentDock = new DocumentDock
        {
            Id = "Documents",
            IsCollapsable = false,
            CanCreateDocument = false,
            CanFloat = false
        };

        Tool leftTool = new() {Id = "Tool1", Title = "Tool 1"};
        Tool bottomTool = new() {Id = "Tool2", Title = "Output"};


        _leftToolDock = new ToolDock
        {
            Id = "LeftPane",
            DockGroup = "Sides",
            CanClose = false,
            Alignment = Alignment.Left,
            Proportion = 0.25,
            VisibleDockables = _factory.CreateList<IDockable>(leftTool),
            ActiveDockable = leftTool
        };

        _rightToolDock = new ToolDock
        {
            Id = "RightPane",
            DockGroup = "Sides",
            CanClose = false,
            Alignment = Alignment.Right,
            Proportion = 0.25,
            VisibleDockables = _factory.CreateList<IDockable>(bottomTool),
            ActiveDockable = bottomTool
        };

        ProportionalDock mainLayout = new()
        {
            Orientation = Orientation.Horizontal,
            VisibleDockables = _factory.CreateList<IDockable>(
                _leftToolDock,
                new ProportionalDockSplitter(),
                _documentDock,
                new ProportionalDockSplitter(),
                _rightToolDock)
        };

        IRootDock root = _factory.CreateRootDock();
        root.VisibleDockables = _factory.CreateList<IDockable>(mainLayout);
        root.DefaultDockable = mainLayout;

        _factory.InitLayout(root);
        _dockControl.Factory = _factory;
        _dockControl.Layout = root;

        Content = _dockControl;
    }

    private void FactoryOnDockableClosing(object? sender, DockableClosingEventArgs e)
    {
        // Little dirty trick to close the view
        // the method is synchronous and opening a dialog to ask for saving is asynchronous
        // so we need to first cancel the event, manage the asynchronous call and then re-enable the event
        // it would be better if the DockableClosing event was asynchronous

        if (e.Dockable is WorkspaceView view)
        {
            e.Cancel = !view.QueryCanClose(() =>
            {
                Dispatcher.UIThread.Post(() =>
                {
                    _factory.CloseDockable(view);
                });
            });
            return;
        }

        e.Cancel = true;
    }

    public event EventHandler<WorkspaceView>? ActiveViewChanged;

    /// <summary>
    ///     Ajoute un panneau (Tool) dans le ToolDock gauche ou droite.
    /// </summary>
    /// <param name="control">Le contrôle à insérer dans le panneau.</param>
    /// <param name="title">Titre du panneau.</param>
    /// <param name="position">Position (gauche/droite)</param>
    public void AddPanel(Control control, string title, PanelPosition position)
    {
        Tool tool = new()
        {
            Id = Guid.NewGuid().ToString(),
            Title = title,
            CanFloat = true,
            CanPin = true,
            CanClose = false,
            Dock = DockMode.Left | DockMode.Right,
            DockGroup = "Sides",
            Content = control
        };
        ToolDock dock = position == PanelPosition.Left ? _leftToolDock : _rightToolDock;

        if (dock.VisibleDockables == null)
        {
            dock.VisibleDockables = _factory.CreateList<IDockable>();
        }

        dock.VisibleDockables.Add(tool);
        dock.ActiveDockable = tool;

        _dockControl.UpdateLayout();
    }

    private void FactoryOnFocusedDockableChanged(object? sender, FocusedDockableChangedEventArgs e)
    {
        if (e.Dockable is WorkspaceView view)
        {
            if (_lastView is { } lastView && lastView != view)
            {
                _lastView.GetContent().Deactivated();
            }

            _lastView = view;

            view.GetContent().Activated();
            ActiveViewChanged?.Invoke(this, view);
        }
    }

    public void GetPanel<T>(out T? panel) where T : class
    {
        panel = null;
        foreach (IDockable dockable in (_leftToolDock?.VisibleDockables ?? Enumerable.Empty<IDockable>())
                 .Concat(_rightToolDock?.VisibleDockables ?? Enumerable.Empty<IDockable>()))
        {
            if (dockable is Tool tool && tool.Content is T match)
            {
                panel = match;
                return;
            }
        }
    }

    public bool IsCurrentDocumentOfType<T>(out T? panel) where T : class
    {
        panel = null;

        if (_documentDock.ActiveDockable is WorkspaceView view && view.Content is T v)
        {
            panel = v;
            return true;
        }

        return false;
    }

    /// <summary>
    ///     Ajoute un document dans la zone centrale, ou l'active si déjà présent.
    /// </summary>
    /// <param name="key">Clé unique du document (Id).</param>
    /// <param name="title">Titre de l'onglet.</param>
    /// <param name="content">Contrôle du document.</param>
    public void OpenDocumentTab(string key, string title, IWorkspaceTabViewBase content)
    {
        if (_documentDock.VisibleDockables == null)
        {
            _documentDock.VisibleDockables = _factory.CreateList<IDockable>();
        }

        // Vérifie si déjà présent via l'Id
        WorkspaceView? document = _documentDock.VisibleDockables.OfType<WorkspaceView>().FirstOrDefault(d => d.Id == key);
        if (document != null)
        {
            _documentDock.ActiveDockable = document;
            return;
        }

        document = new WorkspaceView
        {
            Id = key,
            CanFloat = false,
            Title = title
        };

        document.AttachContent(content);

        _documentDock.VisibleDockables.Add(document);
        _documentDock.ActiveDockable = document;
    }
}