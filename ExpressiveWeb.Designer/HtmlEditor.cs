// *********************************************************
// 
// ExpressiveWeb.Designer HtmlEditor.cs
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

using System.Text;
using System.Text.Json;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Primitives.PopupPositioning;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Threading;
using ExpressiveWeb.Core;
using ExpressiveWeb.Core.Commands;
using ExpressiveWeb.Core.Html;
using ExpressiveWeb.Core.Kit;
using ExpressiveWeb.Core.Log;
using ExpressiveWeb.Core.Network;
using ExpressiveWeb.Designer.Cef;
using ExpressiveWeb.Designer.Commands;
using ExpressiveWeb.Designer.Exceptions;
using ExpressiveWeb.Designer.Filters;
using ExpressiveWeb.Designer.Models;
using ExpressiveWeb.Designer.QuickActions;
using ExpressiveWeb.Designer.Utils;
using ExpressiveWeb.Presentation.MessageBox;
using Microsoft.Extensions.DependencyInjection;
using Xilium.CefGlue;
using Xilium.CefGlue.Avalonia;
using Xilium.CefGlue.Common.Events;

namespace ExpressiveWeb.Designer;

public class HtmlEditor : Border, IDisposable
{
    internal const string JS_GLOBAL_EDITOR_OBJ_NAME = "window.$___CURRENT_EDITOR";
    private readonly AvaloniaCefBrowser _browser = new();
    private readonly HtmlFilterService _htmlFilterService = new();
    private readonly ILogService _logService;
    private readonly INetworkService _networkService;
    private TextEditor? _textEditor;
    private ContextMenu _componentActionsMenu = new();

    private List<IEditorQuickAction> _supportedQuickActions = new()
    {
        new AppendChildQuickAction()
    };

    public HtmlEditor(Kit kit)
    {
        Kit = kit;

        _logService = AppServices.ServicesFactory.GetService<ILogService>();
        _networkService = AppServices.ServicesFactory.GetService<INetworkService>();

        // Set up cleanup filters when exporting HTML from editor
        _htmlFilterService.UseFilter<RemoveEditorInternalIdFilter>();
        _htmlFilterService.UseFilter<RemoveEditorReferencesFilter>();
        _htmlFilterService.UseFilter<RemoveEditorAdornerFilter>();

        // Do not forget to unregister those events in DISPOSE to clear ref
        HtmlEditorCore.UserStylesheetChanged += HtmlEditorCoreOnUserStylesheetChanged;
        _networkService.ModeChanged += OnNetworkModeChanged;

        Initialized += OnInitialized;
    }

    public event EventHandler<object?>? SelectionChanged;
    public event EventHandler<string>? InfoRaised;

    public IBusinessCommandManager CommandManager
    {
        get;
        set;
    } = new BusinessCommandManager();

    internal static Kit Kit
    {
        get;
        private set;
    }

    public HtmlElement? SelectedElement
    {
        get;
        private set;
    }

    public bool IsTextEditing
    {
        get
        {
            return _textEditor != null;
        }
    }

    public void Dispose()
    {
        _networkService.ModeChanged -= OnNetworkModeChanged;
        HtmlEditorCore.UserStylesheetChanged -= HtmlEditorCoreOnUserStylesheetChanged;

        _browser.Dispose();
    }

    private void BrowserOnConsoleMessage(object sender, ConsoleMessageEventArgs message)
    {
        switch (message.Level)
        {
            case CefLogSeverity.Info:
                _logService.Info(message.Message);
                InfoRaised?.Invoke(this, message.Message);
                break;
            case CefLogSeverity.Warning:
            case CefLogSeverity.Error:
                _logService.Error(message.Message);
                break;
        }
    }

    private void BrowserOnLoadEnd(object sender, LoadEndEventArgs e)
    {
        if (e.HttpStatusCode == 404)
        {
            Dispatcher.UIThread.Post(() =>
            {
                Child = new TextBlock {Text = "Unsupported file"};
            });
        }
    }

    private HtmlElement ConvertElementInfoToHtmlElement(HtmlElementInfo info)
    {
        HtmlElement el = new()
        {
            DataContext = info,
            CssClass = info.CssClass,
            TagName = info.TagName,
            InternalId = info.InternalId,
            KitComponent = Kit.Components.FirstOrDefault(x => x.UID.Equals(info.ComponentUid))
        };

        foreach (HTmlElementAttributeInfo infoAttribute in info.Attributes)
        {
            el.Attributes.Add(new HtmlElementAttribute
            {
                Name = infoAttribute.Name,
                Value = infoAttribute.Value
            });
        }

        return el;
    }

    internal async Task EndTextEditing()
    {
        if (_textEditor == null)
        {
            return;
        }

        await _textEditor.Commit();
        _textEditor = null;
    }

    internal async Task<HtmlElementInfo?> GetElementInfoFromInternalId(string internalId)
    {
        string json = await _browser.EvaluateJavaScript<string>(string.Concat("return ", JS_GLOBAL_EDITOR_OBJ_NAME, ".getJsonElementInfoFromInternalId('", internalId, "')"));


        if (string.IsNullOrEmpty(json))
        {
            return null;
        }

        HtmlElementInfo? sourceElementInfo = JsonSerializer.Deserialize<HtmlElementInfo?>(json);

        return sourceElementInfo;
    }

    /// <summary>
    ///     Gets the editor HTML after all cleaning filters have been applied
    /// </summary>
    /// <returns></returns>
    public async Task<string> GetFilteredHtml()
    {
        string html = await _browser.EvaluateJavaScript<string>("return document.querySelector('html').outerHTML");
        html = await _htmlFilterService.FilterAsync(html);
        return html;
    }

    private void HtmlEditorCoreOnUserStylesheetChanged(object? sender, EventArgs e)
    {
    }

    /// <summary>
    ///     Executes the given script in the browser, passing arguments in correct format
    /// </summary>
    /// <param name="methodName"></param>
    /// <param name="args"></param>
    /// <exception cref="ArgumentNullException"></exception>
    internal void InternalCallBrowserMethod(string methodName, params object[] args)
    {
        if (string.IsNullOrEmpty(methodName))
        {
            throw new ArgumentNullException(nameof(methodName));
        }

        methodName = StringEncoding.GetScriptForJavascriptMethodWithArgs(methodName, args);
        _browser.ExecuteJavaScript(methodName);
    }

    /// <summary>
    ///     Executes the given script in the browser
    /// </summary>
    /// <exception cref="ArgumentNullException"></exception>
    internal void InternalExecuteBrowserScript(string script)
    {
        if (string.IsNullOrEmpty(script))
        {
            throw new ArgumentNullException(nameof(script));
        }

        _browser.ExecuteJavaScript(script);
    }

    internal void InvokeContextMenuOpening(double x, double y)
    {
        Dispatcher.UIThread.Post(() =>
        {
            if (ContextMenu != null)
            {
                ContextMenu.CustomPopupPlacementCallback = p =>
                {
                    p.Anchor = PopupAnchor.TopLeft;
                    p.Gravity = PopupGravity.BottomRight;
                    p.Offset = new Point(x + 5, y + 5);
                };

                ContextMenu.Placement = PlacementMode.Custom;
                ContextMenu.Open(this);
            }
        });
    }

    internal void InvokeComponentActionMenuClose()
    {
        if (_componentActionsMenu.IsOpen)
        {
            Dispatcher.UIThread.Post(() =>
            {
                _componentActionsMenu.Close();
            });
        }
    }

    internal void InvokeComponentActionMenuOpening(double x, double y)
    {
        Dispatcher.UIThread.Post(() =>
        {
            if (SelectedElement != null && SelectedElement.KitComponent is { } component)
            {
                BuildComponentActionsMenu(component);

                _componentActionsMenu.CustomPopupPlacementCallback = p =>
                {
                    p.Anchor = PopupAnchor.TopLeft;
                    p.Gravity = PopupGravity.BottomRight;
                    p.Offset = new Point(x, y);
                };

                _componentActionsMenu.PlacementTarget = this;
                _componentActionsMenu.Placement = PlacementMode.Custom;
                _componentActionsMenu.Open(this);
            }
        });
    }

    private void BuildComponentActionsMenu(KitComponent component)
    {
        _componentActionsMenu.Items.Clear();

        foreach (QuickAction quickAction in component.ActionList)
        {
            MenuItem menuItem = new MenuItem
            {
                Header = quickAction.Header,
                Tag = quickAction
            };
            menuItem.Click += MenuItemOnClick;
            _componentActionsMenu.Items.Add(menuItem);
        }
    }

    private void MenuItemOnClick(object? sender, RoutedEventArgs e)
    {
        if (sender == null || sender is not MenuItem menuItem || menuItem.Tag is not QuickAction quickAction)
        {
            return;
        }

        IEditorQuickAction? foundAction = _supportedQuickActions.FirstOrDefault(x => x.CommandName.Equals(quickAction.Command, StringComparison.Ordinal));
        try
        {
            foundAction?.Execute(this, quickAction.Params);
        }
        catch (InvalidQuickActionParameterException ex)
        {
            _ = EWMessageBox.Show(new MessageBoxData()
            {
                Owner = (Application.Current!.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow,
                Buttons = MessageBoxButtons.Ok,
                Message = string.Concat("Invalid action parameter: ", ex.Message),
                Title = "Error"
            });
        }
    }

    internal void InvokeSelectionChanged(HtmlElementInfo? info)
    {
        if (info == null || string.IsNullOrEmpty(info.ComponentUid))
        {
            SelectedElement = null;
            SelectionChanged?.Invoke(this, null);
            return;
        }

        SelectedElement = ConvertElementInfoToHtmlElement(info);
        SelectionChanged?.Invoke(this, SelectedElement);
    }

    internal void InvokeSelectionChanged(TextSelectionInfo? info)
    {
        SelectedElement = null;
        SelectionChanged?.Invoke(this, info);
    }

    public void LoadPage(string filePath)
    {
        // not the best way to do
        _browser.Address = string.Concat(CustomSchemeHandler.LOCAL_FILE_SCHEME, "://p/", Convert.ToBase64String(Encoding.ASCII.GetBytes(filePath)));
        //_browser.Address = "https://test/intercept.html";
    }

    private void OnInitialized(object? sender, EventArgs e)
    {
        _browser.RegisterJavascriptObject(new InternalJavascriptBridge(this, _logService), "$HOST_INTEROP");

        // browser.LoadStart += OnBrowserLoadStart;
        _browser.LoadEnd += BrowserOnLoadEnd;
        _browser.RequestHandler = new CustomRequestHandler();
        _browser.ContextMenuHandler = new CustomContextMenuHandler();
        // browser.TitleChanged += OnBrowserTitleChanged;
        _browser.HorizontalAlignment = HorizontalAlignment.Stretch;
        _browser.VerticalAlignment = VerticalAlignment.Stretch;

        _browser.ConsoleMessage += BrowserOnConsoleMessage;
        Child = _browser;
    }

    private void OnNetworkModeChanged(object? sender, EventArgs e)
    {
        ReloadExternalResources();
    }

    private void ReloadExternalResources()
    {
        InternalExecuteBrowserScript(string.Concat(JS_GLOBAL_EDITOR_OBJ_NAME, ".domHelper.reloadExternalResources()"));
    }

    public void SelectElement(HtmlElement? element)
    {
        SelectElementByInternalId(element?.InternalId);
    }

    public void SelectElementByInternalId(string? internalId)
    {
        if (string.IsNullOrEmpty(internalId))
        {
            InternalExecuteBrowserScript(string.Concat(JS_GLOBAL_EDITOR_OBJ_NAME, ".unSelectAll()"));
        }
        else
        {
            InternalCallBrowserMethod(string.Concat(JS_GLOBAL_EDITOR_OBJ_NAME, ".selectElementByInternalId"), internalId);
        }
    }

    /// <summary>
    /// Unselect all elements in the editor.
    /// </summary>
    public void UnselectAll()
    {
        InternalExecuteBrowserScript(string.Concat(JS_GLOBAL_EDITOR_OBJ_NAME, ".unSelectAll()"));
        SelectedElement = null;
        SelectionChanged?.Invoke(this, null);
    }

    public void ShowDevTools()
    {
        _browser.ShowDeveloperTools();
    }

    internal void StartTextEditing(HtmlElementInfo info)
    {
        _textEditor = new TextEditor(this, info);
        _textEditor.Start();
    }

    internal void UpdateDecorators()
    {
        InternalExecuteBrowserScript(string.Concat(JS_GLOBAL_EDITOR_OBJ_NAME, ".adornerManager.updateDecorators();"));
    }

    public void SelectParentElement()
    {
        if (SelectedElement == null)
        {
            return;
        }

        InternalCallBrowserMethod(string.Concat(JS_GLOBAL_EDITOR_OBJ_NAME, ".selectParentElement"), SelectedElement.InternalId);
    }

    #region Public Commands

    public void SetElementCssClass(string newCssClass)
    {
        ArgumentNullException.ThrowIfNull(SelectedElement, "No element selected");

        HtmlElementInfo info = SelectedElement.DataContext.Freeze();

        if (string.Equals(info.CssClass.Trim(), newCssClass.Trim(), StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        EditElementCssClassCommand cmd = new EditElementCssClassCommand(this)
        {
            InitialElementInfo = info,
            NewCssClass = newCssClass
        };
        CommandManager.ExecuteCommand(cmd);
    }

    public void DuplicateElement()
    {
        ArgumentNullException.ThrowIfNull(SelectedElement, "No element selected");

        HtmlElementInfo info = SelectedElement.DataContext.Freeze();
        info.InternalId = Guid.NewGuid().ToString();
        info.Index = info.Index + 1;

        // The selected element may contain a children tree with existing editor ids
        // we must clear them to ensure everything will be considered as new in Typescript side
        info.InnerHtml = _htmlFilterService.FilterWith<RemoveEditorInternalIdFilter>(info.InnerHtml);

        InsertElementCommand cmd = new(this)
        {
            SourceElementInfo = info
        };
        CommandManager.ExecuteCommand(cmd);
    }

    public void DeleteElement()
    {
        ArgumentNullException.ThrowIfNull(SelectedElement, "No element selected");
        HtmlElementInfo info = SelectedElement.DataContext.Freeze();

        RemoveElementCommand cmd = new(this)
        {
            SourceElementInfo = info
        };
        CommandManager.ExecuteCommand(cmd);
    }

    public void ChangeTextTagType(HtmlDefaultTextTagType newTag)
    {
        ArgumentNullException.ThrowIfNull(SelectedElement, "No element selected");
        HtmlElementInfo info = SelectedElement.DataContext.Freeze();

        string newTagName = string.Empty;

        switch (newTag)
        {
            case HtmlDefaultTextTagType.Paragraph:
                newTagName = "p";
                break;
            case HtmlDefaultTextTagType.Heading1:
                newTagName = "h1";
                break;
            case HtmlDefaultTextTagType.Heading2:
                newTagName = "h2";
                break;
            case HtmlDefaultTextTagType.Heading3:
                newTagName = "h3";
                break;
            case HtmlDefaultTextTagType.Heading4:
                newTagName = "h4";
                break;
            case HtmlDefaultTextTagType.Heading5:
                newTagName = "h5";
                break;
            case HtmlDefaultTextTagType.Heading6:
                newTagName = "h6";
                break;
            case HtmlDefaultTextTagType.Preformatted:
                newTagName = "pre";
                break;
            case HtmlDefaultTextTagType.Quote:
                newTagName = "blockquote";
                break;
            default:
                newTagName = info.TagName;
                break;
        }

        if (newTagName == info.TagName)
        {
            return;
        }

        ChangeElementTagNameCommand cmd = new ChangeElementTagNameCommand(this)
        {
            SourceElementInfo = info,
            NewTagName = newTagName
        };

        CommandManager.ExecuteCommand(cmd);
    }

    #endregion
}