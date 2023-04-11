using Joa.BuiltInPlugin;
using Joa.Step;
using JoaLauncher.Api.Injectables;

namespace Joa.PluginCore;

public class PluginManager
{
    public List<PluginDefinition> Plugins { get; set; } = new();
    public List<ProviderWrapper> GlobalProviders { get; set; } = new();

    private readonly PluginLoader _pluginLoader;
    private readonly IJoaLogger _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly JoaManager _joaManager;

    public PluginManager(PluginLoader pluginLoader, IJoaLogger logger, IServiceProvider serviceProvider, JoaManager joaManager)
    {
        _pluginLoader = pluginLoader;
        _logger = logger;
        _serviceProvider = serviceProvider;
        _joaManager = joaManager;
        ReloadPlugins();
        
        Task.Run(async () =>
        {
            var periodicTimer= new PeriodicTimer(TimeSpan.FromMinutes(5));
            while (await periodicTimer.WaitForNextTickAsync())
            {
                await UpdateIndexesAsync();
            }
        });
    }

    private void ReloadPlugins()
    {
        var builtInProvider = new BuiltInProvider(_serviceProvider, _joaManager, this);
        
        Plugins = _pluginLoader.ReloadPlugins();
        GlobalProviders = Plugins.SelectMany(x => x.GlobalProviders).ToList();
        GlobalProviders.Add(new ProviderWrapper
        {
            Provider = builtInProvider
        });
    }

    public async Task UpdateIndexesAsync()
    {
        _logger.TimedOperation(nameof(UpdateIndexesAsync));
        
        foreach (var cache in Plugins.SelectMany(x => x.Caches))
        {
            try
            {
                cache.UpdateIndexes();
            }
            catch (Exception e)
            {
                _logger.LogException(e, $"Updating the index for cache {cache.GetType().Name} failed");
            }
        }

        foreach (var asyncCache in Plugins.SelectMany(x => x.AsyncCaches))
        {
            try
            {
                await asyncCache.UpdateIndexesAsync();
            }
            catch (Exception e)
            {
                _logger.LogException(e, $"Updating the index for async cache {asyncCache.GetType().Name} failed");
            }
        }
    }
}