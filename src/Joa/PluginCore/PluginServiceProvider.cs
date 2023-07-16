using Joa.Injectables;
using JoaLauncher.Api.Injectables;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TolggeUI;

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
            builder.AddProvider(new TolggeLoggerProvider());
        });
        ServiceCollection.AddSingleton<IIconHelper, IconHelper>();
        ServiceProvider = ServiceCollection.BuildServiceProvider();
    }

    public void BuildServiceProvider()
    {
        ServiceProvider = ServiceCollection.BuildServiceProvider();
    }
}