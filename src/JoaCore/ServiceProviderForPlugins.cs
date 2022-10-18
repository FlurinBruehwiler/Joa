using JoaCore.Settings;
using JoaPluginsPackage.Injectables;
using Microsoft.Extensions.DependencyInjection;

namespace JoaCore;

public class ServiceProviderForPlugins
{
    private readonly SettingsProvider _settingsProvider;
    private readonly IJoaLogger _joaLogger;

    public ServiceProviderForPlugins(SettingsProvider settingsProvider, IJoaLogger joaLogger)
    {
        _settingsProvider = settingsProvider;
        _joaLogger = joaLogger;
        ServiceProvider = CreateServiceProvider();
    }

    public IServiceProvider ServiceProvider { get; set; }
    
    private IServiceProvider CreateServiceProvider()
    {
        var services = new ServiceCollection();
        services.AddSingleton<ISettingsProvider>(_settingsProvider);
        services.AddSingleton(_joaLogger);
        services.AddSingleton<IBrowserHelper, BrowserHelper>();
        services.AddSingleton<IIconHelper, IconHelper>();
        return services.BuildServiceProvider();
    }
}