// *********************************************************
// 
// ExpressiveWeb Program.cs
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
using System.IO;
using Avalonia;
using ExpressiveWeb.Designer.Cef;
using Xilium.CefGlue;
using Xilium.CefGlue.Common;
using Xilium.CefGlue.Common.Shared;

namespace ExpressiveWeb;

internal class Program
{
    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
    {
        string cachePath = Path.Combine(Path.GetTempPath(), "CefGlue_" + Guid.NewGuid().ToString().Replace("-", null));


        return AppBuilder.Configure<App>()
            .UsePlatformDetect().AfterSetup(_ => CefRuntimeLoader.Initialize(new CefSettings
                {
                    RootCachePath = "",
#if WINDOWLESS
                          // its recommended to leave this off (false), since its less performant and can cause more issues
                          WindowlessRenderingEnabled = true
#else
                    WindowlessRenderingEnabled = false
#endif
                },
                customSchemes: new[]
                {
                    new CustomScheme
                    {
                        SchemeName = "pfinternal",
                        SchemeHandlerFactory = new CustomSchemeHandler()
                    }
                })).WithInterFont()
            .LogToTrace();
    }

    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
    }
}