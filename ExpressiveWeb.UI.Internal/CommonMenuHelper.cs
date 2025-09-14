// *********************************************************
// 
// ExpressiveWeb.UI.Internal CommonMenuHelper.cs
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
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Media;
using ExpressiveWeb.Core.ApplicationCommands;
using ExpressiveWeb.Presentation.Menus;

namespace ExpressiveWeb.Presentation;

internal static class CommonMenuHelper
{
    internal static object BuildToolbarItem(Control owner, ApplicationCommandBase cmd)
    {
        if (cmd.CommandName.Equals("-"))
        {
            EWToolbarSeparator sep = new();
            return sep;
        }
        
        EWToolbarButton result = new()
        {
            DataContext = cmd
        };
        
        
        result.Text = cmd.Title;

        result.IconBrush = !string.IsNullOrEmpty(cmd.IconResourceName)
            ? (Brush?) owner.FindResource(cmd.IconResourceName)
            : Brushes.Transparent;

        result.Bind(InputElement.IsEnabledProperty, new Binding(nameof(ApplicationCommandBase.IsEnabled)));
        //result.Bind(EWButton.IsCheckedProperty, new Binding(nameof(ApplicationCommandBase.IsChecked)));
        
        result.Click += (_, _) =>
        {
            if (cmd.HasSubCommands)
            {
                return;
            }

            cmd.Execute();
        };

        return result;
    }

    internal static object BuildMenuItem( Control owner, ApplicationCommandBase cmd, bool allowEnableStateAsVisibility = false)
    {
        if (cmd.CommandName.Equals("-"))
        {
            Separator sep = new();

            // if (cmd is ISetEnableStateAsVisibilityCommand)
            // {
            //     Binding visibilityBinding = new()
            //     {
            //         Path = new PropertyPath(nameof(ApplicationCommandBase.IsEnabled)),
            //         Converter = BooleanToVisibilityConverter,
            //         ConverterParameter = cmd.IsEnabled
            //     };
            //     sep.SetBinding(UIElement.VisibilityProperty, visibilityBinding);
            // }

            return sep;
        }

        EWMenuItem result = new()
        {
            DataContext = cmd
        };


        result.Header = cmd.Title;

        // result.IconBrush = !string.IsNullOrEmpty(cmd.IconResourceName)
        //     ? (Brush?) owner.FindResource(cmd.IconResourceName)
        //     : Brushes.Transparent;

        result.Bind(InputElement.IsEnabledProperty, new Binding(nameof(ApplicationCommandBase.IsEnabled)));
        result.Bind(MenuItem.IsCheckedProperty, new Binding(nameof(ApplicationCommandBase.IsChecked)));

        // if (allowEnableStateAsVisibility && cmd is ISetEnableStateAsVisibilityCommand)
        // {
        //     Binding visibilityBinding = new()
        //     {
        //         Path = new PropertyPath(nameof(ApplicationCommandBase.IsEnabled)),
        //         Converter = BooleanToVisibilityConverter,
        //         ConverterParameter = cmd.IsEnabled
        //     };
        //     result.SetBinding(UIElement.VisibilityProperty, visibilityBinding);
        // }


        // Binding inputGestureBinding = new()
        // {
        //     Path = new PropertyPath(nameof(ApplicationCommandBase.GestureText)),
        //     Converter = KeyMapConverter2,
        //     ConverterParameter = cmd.CommandName
        // };
        // result.SetBinding(MenuItem.InputGestureTextProperty, inputGestureBinding);

        result.Click += (_, _) =>
        {
            if (cmd.HasSubCommands)
            {
                return;
            }

            cmd.Execute();
        };


        return result;
    }
}