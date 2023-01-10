using Joa.Injectables;
using JoaLauncher.Api.Injectables;
using Microsoft.Extensions.DependencyInjection;

namespace Joa.PluginCore;

public class PluginServiceProvider
{
    public IServiceProvider ServiceProvider { get; set; }

    public ServiceCollection ServiceCollection { get; set; }

    public PluginServiceProvider(IJoaLogger joaLogger)
    {
        joaLogger.Info(nameof(PluginServiceProvider));
        ServiceCollection = new ServiceCollection();
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