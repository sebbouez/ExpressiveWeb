// *********************************************************
// 
// ExpressiveWeb.UI.Internal ExplorerIconConverter.cs
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
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Media;
using ExpressiveWeb.Core.Project;

namespace ExpressiveWeb.Modules.Explorer;

public class ExplorerIconConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is ProjectItemType t)
        {
            string iconName = string.Empty;

            switch (t)
            {
                case ProjectItemType.Folder:
                    iconName = "IconFolder";
                    break;
                case ProjectItemType.File:
                    iconName = "IconExplorerPage";
                    break;
                case ProjectItemType.WebReference:
                    iconName = "IconPackage";
                    break;
                case ProjectItemType.Project:
                    iconName = "IconProject";
                    break;
                case ProjectItemType.MasterPage:
                    iconName = "IconExplorerMasterPage";
                    break;
                case ProjectItemType.StyleSheet:
                    iconName = "IconExplorerStylesheet";
                    break;
                case ProjectItemType.Script:
                    iconName = "IconExplorerScript";
                    break;
                case ProjectItemType.ReservedFolder:
                    iconName = "IconExplorerProjectObjects";
                    break;
            }

            if (!string.IsNullOrEmpty(iconName) && Application.Current!.TryGetResource(iconName, null, out object? resource))
            {
                return resource;
            }
        }


        // Valeur par défaut si pas trouvé
        return Brushes.Transparent;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}