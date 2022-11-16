using JoaInterface.PluginCore;
using JoaInterface.Settings;
using JoaInterface.Step;
using Microsoft.Extensions.DependencyInjection;

namespace JoaInterface;

public class JoaManager : IDisposable
{
    public IServiceScope CurrentScope { get; set; }
    private readonly IServiceProvider _serviceProvider;
    
    public JoaManager()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddScoped<Search>();
        serviceCollection.AddScoped<PluginManager>();
        serviceCollection.AddScoped<PluginLoader>();
        serviceCollection.AddScoped<SettingsManager>();
        serviceCollection.AddScoped<PluginServiceProvider>();
        serviceCollection.AddScoped<StepsManager>();
        _serviceProvider = serviceCollection.BuildServiceProvider();
    }

    public void NewScope()
    {
        CurrentScope.Dispose();
        
        var serviceScopefactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

        CurrentScope = serviceScopefactory.CreateScope();
        
        CurrentScope.ServiceProvider.GetService<Search>();
    }

    public void Dispose()
    {
        CurrentScope.Dispose();
    }
}