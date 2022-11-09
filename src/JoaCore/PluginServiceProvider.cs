using JoaCore.Settings;
using JoaPluginsPackage.Injectables;
using Microsoft.Extensions.DependencyInjection;

namespace JoaCore;

public class PluginServiceProvider
{
    public IServiceProvider ServiceProvider { get; set; }
    
    public ServiceCollection ServiceCollection { get; set; }
    
    public PluginServiceProvider(SettingsProvider settingsProvider, IJoaLogger joaLogger)
    {
        ServiceCollection = new ServiceCollection();
        ServiceCollection.AddSingleton<ISettingsProvider>(settingsProvider);
        ServiceCollection.AddSingleton(joaLogger);
        ServiceCollection.AddSingleton<IBrowserHelper, BrowserHelper>();
        ServiceCollection.AddSingleton<IIconHelper, IconHelper>();
        ServiceProvider = ServiceCollection.BuildServiceProvider();
    }

    public void BuildServiceProvider()
    {
        ServiceProvider = ServiceCollection.BuildServiceProvider();
    }
}