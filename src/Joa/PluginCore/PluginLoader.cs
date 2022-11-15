using System.Reflection;
using JoaLauncher.Api;
using JoaLauncher.Api.Injectables;
using JoaLauncher.Api.Plugin;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace JoaInterface.PluginCore;

public class PluginLoader
{
    private readonly IConfiguration _configuration;
    private readonly IJoaLogger _logger;
    private readonly PluginServiceProvider _pluginServiceProvider;
    private Dictionary<Type, object> _instantiatedTypes = new();

    public PluginLoader(IConfiguration configuration, IJoaLogger logger, PluginServiceProvider pluginServiceProvider)
    {
        logger.Info(nameof(PluginLoader));
        _configuration = configuration;
        _logger = logger;
        _pluginServiceProvider = pluginServiceProvider;
    }

    public List<PluginDefinition> ReloadPlugins()
    {
        List<PluginDefinition> pluginDefinitions = new();
        _instantiatedTypes = new Dictionary<Type, object>();
        var assemblies = LoadAssemblies(GetPluginDllPaths(_configuration));
        var pluginTypes = LoadTypes(assemblies);
        
        foreach (var pluginType in pluginTypes)
        {
            var setting = pluginType.Setting is null ? new EmptySetting() : InstantiateSettings(pluginType.Setting);
            var caches = InstantiateCaches(pluginType.Caches).ToList();
            var plugin = InstantiatePlugin(pluginType.Plugin!);
            if(plugin is null)
                continue;
            var pluginBuilder = new PluginBuilder(this, _logger, _pluginServiceProvider);
            pluginDefinitions.Add(pluginBuilder.BuildPluginDefinition(plugin, setting, caches));
        }

        return pluginDefinitions;
    }

    public bool TryGetExistingObject<T>(Type type, out T? obj) where T : class
    {
        if (_instantiatedTypes.TryGetValue(type, out var x))
        {
            obj = (T)x;
            return true;
        }
        obj = null;
        return false;
    }
    
    private ISetting InstantiateSettings(Type settingType)
    {
        if (TryGetExistingObject<ISetting>(settingType, out var s))
            return s!;
        
        if (ActivatorUtilities.CreateInstance(_pluginServiceProvider.ServiceProvider, settingType) is not ISetting
            setting)
        {
            _logger.Error($"{settingType.Name} does not inherit from {nameof(ISetting)}");
            return new EmptySetting();
        }

        _instantiatedTypes.Add(settingType, setting);
        _pluginServiceProvider.ServiceCollection.AddSingleton(setting.GetType(), setting);
        _pluginServiceProvider.BuildServiceProvider();

        return setting;
    }
    
    private IEnumerable<ICache> InstantiateCaches(List<Type> cacheTypes)
    {
        foreach (var cacheType in cacheTypes)
        {
            if (TryGetExistingObject<ICache>(cacheType, out var c))
            {
                yield return c!;
                continue;
            }
            
            if(ActivatorUtilities.CreateInstance(_pluginServiceProvider.ServiceProvider, cacheType) is not ICache cache)
                continue;

            _instantiatedTypes.Add(cacheType, cache);
            _pluginServiceProvider.ServiceCollection.AddSingleton(cache.GetType(), cache);

            yield return cache;
        }
        
        _pluginServiceProvider.BuildServiceProvider();
    }

    private IPlugin? InstantiatePlugin(Type pluginType)
    {
        if (TryGetExistingObject<IPlugin>(pluginType, out var p))
            return p!;

        if (ActivatorUtilities.CreateInstance(_pluginServiceProvider.ServiceProvider, pluginType) is not IPlugin plugin)
            return null;
        
        _instantiatedTypes.Add(pluginType, plugin);
        return plugin;
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

            if (typeof(ISetting).IsAssignableFrom(type))
            {
                if (pluginTypes.Setting is not null)
                {
                    _logger.Error("A plugin can only contain one Setting");
                    continue;
                }
                    
                pluginTypes.Setting = type;
            }
        }

        if (pluginTypes.Plugin is not null)
            return pluginTypes;
        
        var availableTypes = string.Join("\n", assembly.GetTypes().Select(t => t.FullName));
        _logger.Log($"Can't find any type which implements IPlugin in {assembly} from {assembly.Location}.\n" +
                    $"Available types: \n{availableTypes}", IJoaLogger.LogLevel.Warning);
        return null;
    }

    private List<string> GetPluginDllPaths(IConfiguration configuration)
    {
        var path = configuration.GetValue<string>("PluginLocation") ?? throw new Exception();
        
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