using Joa.BuiltInPlugin;
using Joa.Hotkey;
using Joa.Injectables;
using Joa.PluginCore;
using Joa.Settings;
using Joa.UI;
using Joa.UI.Settings;
using JoaKit;
using JoaLauncher.Api.Injectables;
using Microsoft.Extensions.DependencyInjection;
using Modern.WindowKit;
using Modern.WindowKit.Controls;
using Modern.WindowKit.Platform;
using SkiaSharp;

namespace Joa;

public static class Program
{
    [STAThread]
    public static void Main()
    {
        var builder = JoaKitApp.CreateBuilder();

        builder.Services.AddSingleton<IJoaLogger>(JoaLogger.GetInstance());
        builder.Services.AddSingleton<JoaManager>();
        builder.Services.AddSingleton<FileSystemManager>();

        builder.Services.AddScoped<Search>();
        builder.Services.AddScoped<PluginManager>();
        builder.Services.AddScoped<PluginLoader>();
        builder.Services.AddScoped<SettingsManager>();
        builder.Services.AddScoped<PluginServiceProvider>();
        builder.Services.AddScoped<BuiltInProvider>();
        builder.Services.AddScoped<GlobalHotKey>();
        builder.Services.AddScoped<HotKeyService>();

        builder.Services.Configure<PathsConfiguration>(builder.Configuration.GetSection("Paths"));

        var app = builder.Build();

        app.CreateWindow<SearchBar>(window =>
        {
            window.Resize(new Size(SearchBar.Width, SearchBar.SearchBoxHeight));
            window.SetTitle("Joa");
            window.SetIcon(SKBitmap.Decode("icon.png"));
            window.CanResize(false);
            window.SetExtendClientAreaTitleBarHeightHint(0);
            window.SetExtendClientAreaToDecorationsHint(true);
            window.SetExtendClientAreaChromeHints(ExtendClientAreaChromeHints.NoChrome);
            window.ShowTaskbarIcon(false);
            CenterWindow(window);
        }, false);

        app.Run();
    }

    private static void CenterWindow(IWindowImpl window)
    {
        var screens = new Screens(window.Screen);
        var screen = screens.Primary ?? screens.All[0];

        var rect = window.FrameSize.HasValue
            ? new PixelRect(PixelSize.FromSize(window.FrameSize.Value, window.DesktopScaling))
            : new PixelRect(PixelSize.FromSize(window.ClientSize, window.DesktopScaling));

        window.Move(screen.WorkingArea.CenterRect(rect).Position);
    }
}