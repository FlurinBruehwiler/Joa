using Joa.Injectables;
using JoaKit;
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
            builder.AddProvider(new JoaKitLoggerProvider());
        });
        ServiceCollection.AddSingleton<IIconHelper, IconHelper>();
        ServiceProvider = ServiceCollection.BuildServiceProvider();
    }

    public void BuildServiceProvider()
    {
        ServiceProvider = ServiceCollection.BuildServiceProvider();
    }
}