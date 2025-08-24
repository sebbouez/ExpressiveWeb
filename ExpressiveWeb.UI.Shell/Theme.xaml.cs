using Avalonia.Markup.Xaml;
using Avalonia.Styling;

namespace ExpressiveWeb.UI.Shell;

public class ExpressiveWebTheme : Styles
{
   
    public ExpressiveWebTheme(IServiceProvider? sp = null)
    {
        AvaloniaXamlLoader.Load(sp, this);
    }
}