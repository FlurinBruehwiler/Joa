using Joa.Hotkey;
using Joa.Injectables;
using Joa.PluginCore;
using Joa.Settings;
using Joa.UI.Search;
using Joa.UI.Settings;
using JoaLauncher.Api.Injectables;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Photino.Blazor;

namespace Joa;

public class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .AddCommandLine(args)
            .Build();

        var serviceCollection = new ServiceCollection();

        serviceCollection.AddSingleton<IJoaLogger>(JoaLogger.GetInstance());
        serviceCollection.AddSingleton<JoaManager>();
        serviceCollection.AddSingleton<FileSystemManager>();

        serviceCollection.AddScoped<Search>();
        serviceCollection.AddScoped<PluginManager>();
        serviceCollection.AddScoped<PluginLoader>();
        serviceCollection.AddScoped<SettingsManager>();
        serviceCollection.AddScoped<PluginServiceProvider>();
        serviceCollection.AddScoped<BuiltInProvider>();
        serviceCollection.AddScoped<HotKeyService>();

        serviceCollection.Configure<PathsConfiguration>(configuration.GetSection("Paths"));

        var serviceProvider = serviceCollection.BuildServiceProvider();

        var joaManager = serviceProvider.GetRequiredService<JoaManager>();
        joaManager.NewScope();

        CreateSearchWindow(serviceProvider);
    }

    private static void CreateSearchWindow(IServiceProvider serviceProvider)
    {
        var searchWindowBuilder = PhotinoBlazorAppBuilder.CreateDefault();

        searchWindowBuilder.Services.AddSingleton(serviceProvider.GetRequiredService<IJoaLogger>());
        searchWindowBuilder.Services.AddSingleton(serviceProvider.GetRequiredService<JoaManager>());
        searchWindowBuilder.Services.AddSingleton(serviceProvider.GetRequiredService<FileSystemManager>());

        searchWindowBuilder.RootComponents.Add<SearchWrapper>("app");

        var searchWindow = searchWindowBuilder.Build();

        searchWindow.MainWindow
            .SetIconFile("favicon.ico")
            .SetTitle("Joa")
            .SetSize(600, 90)
            .SetUseOsDefaultSize(false)
            .SetResizable(false)
            .SetMinimized(true)
            .SetChromeless(true);

        AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
        {
            JoaLogger.GetInstance().Error(args.ExceptionObject.ToString());
        };

        searchWindow.Run();
    }
    
    public static void CreateSettingsWindow(IServiceProvider serviceProvider)
    {
        var settingsWindowBuilder = PhotinoBlazorAppBuilder.CreateDefault();

        settingsWindowBuilder.Services.AddSingleton(serviceProvider.GetRequiredService<IJoaLogger>());
        settingsWindowBuilder.Services.AddSingleton(serviceProvider.GetRequiredService<JoaManager>());
        settingsWindowBuilder.Services.AddSingleton(serviceProvider.GetRequiredService<FileSystemManager>());

        settingsWindowBuilder.RootComponents.Add<SettingsComponent>("app");

        var settingsWindow = settingsWindowBuilder.Build();

        settingsWindow.MainWindow
            .SetIconFile("favicon.ico")
            .SetTitle("Joa")
            .SetSize(1000, 1000);

        AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
        {
            JoaLogger.GetInstance().Error(args.ExceptionObject.ToString());
        };

        settingsWindow.Run();
    }
}


