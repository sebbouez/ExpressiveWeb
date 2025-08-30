// *********************************************************
// 
// ExpressiveWeb.UI.Internal EWMainToolbar.cs
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
using Avalonia.Controls.Primitives;
using ExpressiveWeb.Core.ApplicationCommands;

namespace ExpressiveWeb.Presentation.Menus;

public class EWMainToolbar : TemplatedControl
{
    private EWToolbar? _centerToolbar;
    private EWToolbar? _leftToolbar;
    private EWToolbar? _rightToolbar;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        _leftToolbar = e.NameScope.Find<EWToolbar>("PART_ToolbarLeft");
        _centerToolbar = e.NameScope.Find<EWToolbar>("PART_ToolbarCenter");
        _rightToolbar = e.NameScope.Find<EWToolbar>("PART_ToolbarRight");
    }

    public void SetCenterToolbarItems(List<ApplicationCommandBase?> commands)
    {
        List<object> items = new();

        foreach (ApplicationCommandBase? command in commands)
        {
            if (command == null)
            {
                continue;
            }

            items.Add(CommonMenuHelper.BuildToolbarItem(this, command));
        }

        _centerToolbar!.Items = items;
    }

    public void SetLeftToolbarItems(List<ApplicationCommandBase?> commands)
    {
        List<object> items = new();

        foreach (ApplicationCommandBase? command in commands)
        {
            if (command == null)
            {
                continue;
            }

            items.Add(CommonMenuHelper.BuildToolbarItem(this, command));
        }

        _leftToolbar!.Items = items;
    }

    public void SetRightToolbarItems(List<ApplicationCommandBase?> commands)
    {
        List<object> items = new();

        foreach (ApplicationCommandBase? command in commands)
        {
            if (command == null)
            {
                continue;
            }

            items.Add(CommonMenuHelper.BuildToolbarItem(this, command));
        }

        _rightToolbar!.Items = items;
    }
}