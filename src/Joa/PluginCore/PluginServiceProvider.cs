using Joa.Injectables;
using JoaLauncher.Api.Injectables;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Joa.PluginCore;

public class PluginServiceProvider
{
    public IServiceProvider ServiceProvider { get; set; }

    public ServiceCollection ServiceCollection { get; set; }

    public PluginServiceProvider(ILogger<PluginServiceProvider> logger)
    {
        ServiceCollection = new ServiceCollection();
        ServiceCollection.AddSingleton(logger); //ToDo
        ServiceCollection.AddSingleton<IBrowserHelper, BrowserHelper>();
        ServiceCollection.AddSingleton<IIconHelper, IconHelper>();
        ServiceCollection.AddSingleton<IClipboardHelper, ClipboardHelper>();
        ServiceProvider = ServiceCollection.BuildServiceProvider();
    }

    public void BuildServiceProvider()
    {
        ServiceProvider = ServiceCollection.BuildServiceProvider();
    }
}