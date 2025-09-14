// *********************************************************
// 
// ExpressiveWeb.UI.Internal EWTabPivot.cs
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
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;

namespace ExpressiveWeb.Presentation.Pivot;

public class EWTabPivot : TabControl
{
    private ItemsPresenter? _itemsPresenter;

    public EWTabPivot()
    {
        Loaded += OnLoaded;
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _itemsPresenter = e.NameScope.Find<ItemsPresenter>("PART_ItemsPresenter");
    }

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        UpdateUniformGridColumns();
    }

    private void UpdateUniformGridColumns()
    {
        if (_itemsPresenter?.Panel is UniformGrid uniformGrid)
        {
            uniformGrid.Columns = Items.Count;
        }
    }
}