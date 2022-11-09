using System.Reflection;
using JoaPluginsPackage;
using JoaPluginsPackage.Injectables;
using JoaPluginsPackage.Plugin;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace JoaCore.PluginCore;

public class PluginLoader
{
    private readonly IConfiguration _configuration;
    private readonly IJoaLogger _logger;
    private readonly PluginServiceProvider _pluginServiceProvider;

    public PluginLoader(IConfiguration configuration, IJoaLogger logger, PluginServiceProvider pluginServiceProvider)
    {
        _configuration = configuration;
        _logger = logger;
        _pluginServiceProvider = pluginServiceProvider;
    }

    public List<PluginDefinition> ReloadPlugins()
    {
        List<PluginDefinition> pluginDefinitions = new();
        var assemblies = LoadAssemblies(GetPluginDllPaths(_configuration));
        var pluginTypes = LoadTypes(assemblies);
        
        foreach (var pluginType in pluginTypes)
        {
            var settings = InstantiateSettings(pluginType.Settings).ToList();
            var caches = InstantiateCaches(pluginType.Caches).ToList();
            var plugin = InstantiatePlugin(pluginType.Plugin!);
            if(plugin is null)
                continue;
            var pluginBuilder = new PluginBuilder(_logger, _pluginServiceProvider);
            pluginDefinitions.Add(pluginBuilder.BuildPluginDefinition(plugin, settings, caches));
        }

        return pluginDefinitions;
    }
    
    private IEnumerable<ISetting> InstantiateSettings(List<Type> settingTypes)
    {
        foreach (var settingType in settingTypes)
        {
            if (ActivatorUtilities.CreateInstance(_pluginServiceProvider.ServiceProvider, settingType) is not ISetting setting)
                continue;

            _pluginServiceProvider.ServiceCollection.AddSingleton(setting);
            
            yield return setting;
        }
        
        _pluginServiceProvider.BuildServiceProvider();
    }
    
    private IEnumerable<ICache> InstantiateCaches(List<Type> cacheTypes)
    {
        foreach (var cacheType in cacheTypes)
        {
            if(ActivatorUtilities.CreateInstance(_pluginServiceProvider.ServiceProvider, cacheType) is not ICache cache)
                continue;

            _pluginServiceProvider.ServiceCollection.AddSingleton(cache);

            yield return cache;
        }
        
        _pluginServiceProvider.BuildServiceProvider();
    }

    private IPlugin? InstantiatePlugin(Type pluginType)
    {
        if (ActivatorUtilities.CreateInstance(_pluginServiceProvider.ServiceProvider, pluginType) is IPlugin plugin)
            return plugin;

        return null;
    }

    private List<PluginTypes> LoadTypes(List<Assembly> assemblies)
    {
        return assemblies.Select(LoadType).Where(type => type is not null).ToList()!;
    }

    private PluginTypes? LoadType(Assembly assembly)
    {
        PluginTypes pluginTypes = new();
        
        foreach (var type in assembly.GetTypes())
        {
            if (typeof(IPlugin).IsAssignableFrom(type))
                pluginTypes.Plugin = type;
            
            if (typeof(ICache).IsAssignableFrom(type))
                pluginTypes.Caches.Add(type);
            
            if(typeof(ISetting).IsAssignableFrom(type))
                pluginTypes.Settings.Add(type);
        }

        if (pluginTypes.Plugin is not null)
            return pluginTypes;
        
        var availableTypes = string.Join(",", assembly.GetTypes().Select(t => t.FullName));
        _logger.Log($"Can't find any type which implements IPlugin in {assembly} from {assembly.Location}.\n" +
                    $"Available types: {availableTypes}", IJoaLogger.LogLevel.Warning);
        return null;
    }

    private List<string> GetPluginDllPaths(IConfiguration configuration)
    {
        var path = configuration.GetValue<string>("PluginLocation");
        
        var pluginFolder =
            Path.GetFullPath(Path.Combine(typeof(PluginLoader).Assembly.Location, path));

        _logger.Log($"Searching for Plugins in {pluginFolder}", IJoaLogger.LogLevel.Info);

        var pluginFolders = Directory.GetDirectories(pluginFolder);

        var plugins = pluginFolders.Select(x => Directory.GetFiles(x)
                .FirstOrDefault(file => file.EndsWith($"{Directory.GetParent(file)?.Name}.dll"))).Where(x => x != null)
            .ToList();

        var pluginsToLog = plugins.Aggregate("", (current, plugin) => current + Environment.NewLine + plugin);
        _logger.Log($"Found the following plugins DLLs: {pluginsToLog}", IJoaLogger.LogLevel.Info);

        return plugins!;
    }

    private List<Assembly> LoadAssemblies(List<string> dllPaths)
    {
        return dllPaths.Select(LoadAssembly).ToList();
    }

    private Assembly LoadAssembly(string pluginLocation)
    {
        var loadContext = new PluginLoadContext(pluginLocation);
        return loadContext.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(pluginLocation)));
    }
}