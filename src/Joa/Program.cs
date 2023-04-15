using Joa.BuiltInPlugin;
using Joa.Hotkey;
using Joa.Injectables;
using Joa.PluginCore;
using Joa.Settings;
using Joa.UI;
using JoaKit;
using JoaLauncher.Api.Injectables;
using Microsoft.Extensions.DependencyInjection;
using Modern.WindowKit;
using Modern.WindowKit.Platform;
using SkiaSharp;

namespace Joa;

public static class Program
{
    [STAThread]
    public static void Main()
    {
        var builder = JoaKitApp.CreateBuilder();

        builder.AddWindow<TestComponent>(window =>
        {
            window.Resize(new Size(500, 20));
            window.SetTitle("Modern.WindowKit Demo");
            window.SetIcon(SKBitmap.Decode("icon.png"));
            window.CanResize(false);
            window.SetExtendClientAreaTitleBarHeightHint(0);
            window.SetExtendClientAreaToDecorationsHint(true);
            window.SetExtendClientAreaChromeHints(ExtendClientAreaChromeHints.NoChrome);
            window.ShowTaskbarIcon(false);
            window.LostFocus = window.Hide;
        });

        builder.AddWindow<TestComponent>(impl => {});
        
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
        
        var joaManager = app.Services.GetRequiredService<JoaManager>();
        joaManager.NewScope();
        
        app.Run();
    }
}


