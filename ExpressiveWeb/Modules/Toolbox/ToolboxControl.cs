// *********************************************************
// 
// ExpressiveWeb ToolboxControl.cs
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

using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;

namespace ExpressiveWeb.Modules.Toolbox;

public class ToolboxControl : TemplatedControl
{
    private ObservableCollection<ToolboxItem> _items = new();
    private ListBox? _listBox;

    public ObservableCollection<ToolboxItem> Items
    {
        get
        {
            return _items;
        }
        set
        {
            _items = value;
            if (_listBox != null)
            {
                _listBox.ItemsSource = _items;
            }
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        _listBox = e.NameScope.Find<ListBox>("PART_ListBox");
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        if (_items.Count > 0 && _listBox != null && _listBox.ItemsSource == null)
        {
            _listBox.ItemsSource = _items;
        }
    }
}