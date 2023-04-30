using Joa.BuiltInPlugin;
using Joa.Steps;
using JoaLauncher.Api;
using Microsoft.Extensions.Logging;

namespace Joa.PluginCore;

public class PluginManager
{
    public List<PluginDefinition> Plugins { get; set; } = new();
    public List<ProviderWrapper> GlobalProviders { get; set; } = new();

    private readonly PluginLoader _pluginLoader;
    private readonly ILogger<PluginManager> _logger;
    private readonly IServiceProvider _serviceProvider;

    public PluginManager(PluginLoader pluginLoader, ILogger<PluginManager> logger, IServiceProvider serviceProvider)
    {
        _pluginLoader = pluginLoader;
        _logger = logger;
        _serviceProvider = serviceProvider;
        ReloadPlugins();

        Task.Run(async () =>
        {
            var periodicTimer = new PeriodicTimer(TimeSpan.FromMinutes(5));
            while (await periodicTimer.WaitForNextTickAsync())
            {
                await UpdateIndexesAsync();
            }
        });
    }

    private void ReloadPlugins()
    {
        var builtInProvider = new BuiltInProvider(_serviceProvider, this);

        Plugins = _pluginLoader.ReloadPlugins();
        GlobalProviders = Plugins.SelectMany(x => x.GlobalProviders).ToList();
        GlobalProviders.Add(new ProviderWrapper
        {
            Provider = builtInProvider
        });
    }

    public async Task UpdateIndexesAsync()
    {
        using var _ = _logger.TimedLogOperation();

        foreach (var cache in Plugins.SelectMany(x => x.Caches))
        {
            try
            {
                cache.UpdateIndexes();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Updating the index for cache {cacheName} failed", cache.GetType().Name);
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
                _logger.LogError(e, "Updating the index for async cache {asyncCacheName} failed", asyncCache.GetType().Name);
            }
        }
    }
}