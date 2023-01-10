using Joa.Injectables;
using Joa.PluginCore;
using Joa.Settings;
using Joa.UI;
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

        serviceCollection.Configure<PathsConfiguration>(configuration.GetSection("Paths"));

        var serviceProvider = serviceCollection.BuildServiceProvider();

        var joaManager = serviceProvider.GetRequiredService<JoaManager>();
        joaManager.NewScope();

        CreateWindow(serviceProvider);
    }

    private static void CreateWindow(IServiceProvider serviceProvider)
    {
        var newAppBuilder = PhotinoBlazorAppBuilder.CreateDefault();

        newAppBuilder.Services.AddSingleton(serviceProvider.GetRequiredService<IJoaLogger>());
        newAppBuilder.Services.AddSingleton(serviceProvider.GetRequiredService<JoaManager>());
        newAppBuilder.Services.AddSingleton(serviceProvider.GetRequiredService<FileSystemManager>());

        //ToDo
        newAppBuilder.Services.AddLogging();

        newAppBuilder.RootComponents.Add<App>("app");

        var photinoBlazorApp = newAppBuilder.Build();

        photinoBlazorApp.MainWindow
            .SetIconFile("favicon.ico")
            .SetTitle("Joa")
            .SetSize(600, 90)
            .SetUseOsDefaultSize(false)
            .SetResizable(false)
            .SetChromeless(true);

        AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
        {
            JoaLogger.GetInstance().Error(args.ExceptionObject.ToString());
        };

        photinoBlazorApp.Run();
    }
}


