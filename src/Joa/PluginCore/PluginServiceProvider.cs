using FlurinBruehwiler.Helpers;
using Joa.Injectables;
using JoaLauncher.Api.Injectables;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Joa.PluginCore;

public class PluginServiceProvider
{
    public IServiceProvider ServiceProvider { get; set; }

    public ServiceCollection ServiceCollection { get; set; }

    public PluginServiceProvider()
    {
        ServiceCollection = new ServiceCollection();
        ServiceCollection.AddLogging(builder =>
        {
            // builder.AddProvider(new TolggeLoggerProvider());
        });
        ServiceCollection.AddSingleton<IIconHelper, IconHelper>();
        ServiceCollection.AddSingleton<IClipboardHelper, ClipboardHelper>();
        ServiceCollection.AddSingleton<IBrowserHelper, BrowserHelper>();
        ServiceCollection.AddSingleton<IIconHelper, IconHelper>();
        ServiceProvider = ServiceCollection.BuildServiceProvider();
    }

    public void BuildServiceProvider()
    {
        ServiceProvider = ServiceCollection.BuildServiceProvider();
    }
}
