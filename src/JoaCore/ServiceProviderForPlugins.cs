using JoaCore.Settings;
using JoaPluginsPackage.Injectables;
using Microsoft.Extensions.DependencyInjection;

namespace JoaCore;

public class ServiceProviderForPlugins
{
    private readonly CoreSettings _coreSettings;
    private readonly IJoaLogger _joaLogger;

    public ServiceProviderForPlugins(CoreSettings coreSettings, IJoaLogger joaLogger)
    {
        _coreSettings = coreSettings;
        _joaLogger = joaLogger;
        ServiceProvider = CreateServiceProvider();
    }

    public IServiceProvider ServiceProvider { get; set; }
    
    private IServiceProvider CreateServiceProvider()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IJoaSettings>(_coreSettings);
        services.AddSingleton(_joaLogger);
        services.AddSingleton<IBrowserHelper, BrowserHelper>();
        return services.BuildServiceProvider();
    }
}