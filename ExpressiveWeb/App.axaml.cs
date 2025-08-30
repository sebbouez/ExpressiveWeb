// *********************************************************
// 
// ExpressiveWeb App.axaml.cs
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
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using ExpressiveWeb.Core;
using ExpressiveWeb.Core.ApplicationCommands;
using ExpressiveWeb.Core.BackgroundServices;
using ExpressiveWeb.Core.Env;
using ExpressiveWeb.Core.Kit;
using ExpressiveWeb.Core.Libraries;
using ExpressiveWeb.Core.Log;
using ExpressiveWeb.Core.Network;
using ExpressiveWeb.Core.Packages;
using ExpressiveWeb.Core.Project;
using ExpressiveWeb.Core.Settings;
using ExpressiveWeb.Core.Style;
using Microsoft.Extensions.DependencyInjection;

namespace ExpressiveWeb;

public class App : Application
{
    private IServiceProvider ConfigureServices()
    {
        ServiceCollection services = new();
        services.AddSingleton<IEnvironmentService, AppEnvironmentService>();
        services.AddSingleton<IApplicationCommandsService, ApplicationCommandsService>();
        services.AddSingleton<ISettingsService, SettingsService>();
        services.AddSingleton<ILogService, LogService>();
        services.AddSingleton<IBackgroundTaskManager, BackgroundTaskManager>();
        services.AddSingleton<INetworkService, NetworkService>();
        services.AddSingleton<ILibraryService, LibraryService>();

        services.AddTransient<IProjectService, ProjectService>();
        services.AddTransient<IKitService, KitService>();
        services.AddTransient<IPackagesService, PackagesService>();
        services.AddTransient<IStyleService, StyleService>();

        return services.BuildServiceProvider();
    }

    private void DesktopOnExit(object? sender, ControlledApplicationLifetimeExitEventArgs e)
    {
        AppServices.ServicesFactory?.GetService<ILogService>()?.Dispose();
    }

    private UserSettings GetDefaultUserSettings()
    {
        UserSettings settings = new();
        settings.UISettings.MainToolbarLeftCommands.Add("OpenProject");
        settings.UISettings.MainToolbarLeftCommands.Add("-");
        settings.UISettings.MainToolbarLeftCommands.Add("NewPage");
        settings.UISettings.MainToolbarLeftCommands.Add("-");
        settings.UISettings.MainToolbarLeftCommands.Add("Undo");
        settings.UISettings.MainToolbarLeftCommands.Add("Redo");


        settings.UISettings.MainToolbarCenterCommands.Add("DeleteElement");
        settings.UISettings.MainToolbarCenterCommands.Add("DuplicateElement");
        settings.UISettings.MainToolbarLeftCommands.Add("-");
        settings.UISettings.MainToolbarCenterCommands.Add("SelectParentElement");

        return settings;
    }

    public override void Initialize()
    {
        Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
        Trace.AutoFlush = true;

        AvaloniaXamlLoader.Load(this);

        AppServices.ServicesFactory = ConfigureServices();

        if (!AppServices.ServicesFactory.GetService<IEnvironmentService>()!.IsValid)
        {
            throw new Exception("Cannot initialize services");
        }
    }

    public override void OnFrameworkInitializationCompleted()
    {
        AppServices.ServicesFactory!.GetService<ISettingsService>()?.LoadSettings(GetDefaultUserSettings());

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            MainWindow mainWindow = new();
            AppState.Instance.AppWindow = mainWindow;
            desktop.MainWindow = mainWindow;
            desktop.Exit += DesktopOnExit;
        }

        base.OnFrameworkInitializationCompleted();
    }
}