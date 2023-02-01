using System.Reflection;
using System.Runtime.Loader;
using JoaLauncher.Api;
using JoaLauncher.Api.Injectables;
using JoaLauncher.Api.Plugin;
using Microsoft.Extensions.DependencyInjection;

namespace Joa.PluginCore;

public class PluginLoader
{
    private readonly IJoaLogger _logger;
    private readonly PluginServiceProvider _pluginServiceProvider;
    private Dictionary<Type, object> _instantiatedTypes = new();
    private readonly FileSystemManager _fileSystemManager;

    public AssemblyLoadContext? AssemblyLoadContext { get; private set; }

    public PluginLoader(IJoaLogger logger, PluginServiceProvider pluginServiceProvider,
        FileSystemManager fileSystemManager)
    {
        logger.Info(nameof(PluginLoader));
        _logger = logger;
        _pluginServiceProvider = pluginServiceProvider;
        _fileSystemManager = fileSystemManager;
    }

    public List<PluginDefinition> ReloadPlugins()
    {
        var pluginsFinalLocation = _fileSystemManager.GetPluginsFinalLocation();
        var pluginsLocation = _fileSystemManager.GetPluginsLocation();

        ClearPluginsFinalLocation(pluginsFinalLocation);
        _logger.Info($"Moving plugins from {pluginsLocation} to {pluginsFinalLocation}");
        MovePluginDllsToCopyLocation(pluginsLocation, pluginsFinalLocation);

        List<PluginDefinition> pluginDefinitions = new();
        _instantiatedTypes = new Dictionary<Type, object>();
        var assemblies = LoadAssemblies(GetPluginDllPaths(pluginsFinalLocation));
        var pluginTypes = LoadTypes(assemblies);

        foreach (var pluginType in pluginTypes)
        {
            var setting = pluginType.Setting is null ? new EmptySetting() : InstantiateSettings(pluginType.Setting);
            var caches = InstantiateCaches(pluginType.Caches).ToList();
            var asyncCaches = InstantiateAsyncCaches(pluginType.AsyncCaches).ToList();
            var plugin = InstantiatePlugin(pluginType.Plugin!);
            if (plugin is null)
                continue;
            var pluginBuilder = new PluginBuilder(this, _logger, _pluginServiceProvider);
            pluginDefinitions.Add(pluginBuilder.BuildPluginDefinition(plugin, setting, caches, asyncCaches));
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

    private void ClearPluginsFinalLocation(string pluginsFinalLocation)
    {
        var di = new DirectoryInfo(pluginsFinalLocation);

        foreach (var file in di.GetFiles())
        {
            file.Delete();
        }

        foreach (var dir in di.GetDirectories())
        {
            dir.Delete(true);
        }
    }

    private void MovePluginDllsToCopyLocation(string pluginLocation, string pluginsFinalLocation)
    {
        var dir = new DirectoryInfo(pluginLocation);

        if (!dir.Exists)
            throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

        var dirs = dir.GetDirectories();

        Directory.CreateDirectory(pluginsFinalLocation);

        foreach (var file in dir.GetFiles())
        {
            var targetFilePath = Path.Combine(pluginsFinalLocation, file.Name);
            file.CopyTo(targetFilePath);
        }

        foreach (var subDir in dirs)
        {
            var newDestinationDir = Path.Combine(pluginsFinalLocation, subDir.Name);
            MovePluginDllsToCopyLocation(subDir.FullName, newDestinationDir);
        }
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

            if (ActivatorUtilities.CreateInstance(_pluginServiceProvider.ServiceProvider,
                    cacheType) is not ICache cache)
                continue;

            _instantiatedTypes.Add(cacheType, cache);
            _pluginServiceProvider.ServiceCollection.AddSingleton(cache.GetType(), cache);

            yield return cache;
        }

        _pluginServiceProvider.BuildServiceProvider();
    }

    private IEnumerable<IAsyncCache> InstantiateAsyncCaches(List<Type> asyncCacheTypes)
    {
        foreach (var cacheType in asyncCacheTypes)
        {
            if (TryGetExistingObject<IAsyncCache>(cacheType, out var c))
            {
                yield return c!;
                continue;
            }

            if (ActivatorUtilities.CreateInstance(_pluginServiceProvider.ServiceProvider,
                    cacheType) is not IAsyncCache cache)
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

            if (typeof(IAsyncCache).IsAssignableFrom(type))
                pluginTypes.AsyncCaches.Add(type);

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
                    $"Available types: \n{availableTypes}", LogLevel.Warning);
        return null;
    }

    private List<string> GetPluginDllPaths(string pluginsFinalLocation)
    {
        _logger.Log($"Searching for Plugins in {pluginsFinalLocation}", LogLevel.Info);

        var pluginFolders = Directory.GetDirectories(pluginsFinalLocation);

        var plugins = pluginFolders.Select(x => Directory.GetFiles(x)
                .FirstOrDefault(file => file.EndsWith($"{Directory.GetParent(file)?.Name}.dll"))).Where(x => x != null)
            .ToList();

        var pluginsToLog = plugins.Aggregate("", (current, plugin) => current + Environment.NewLine + plugin);
        _logger.Log($"Found the following plugins DLLs: {pluginsToLog}", LogLevel.Info);

        return plugins!;
    }

    private List<Assembly> LoadAssemblies(List<string> dllPaths)
    {
        return dllPaths.Select(LoadAssembly).ToList();
    }

    private Assembly LoadAssembly(string pluginLocation)
    {
        AssemblyLoadContext = new PluginLoadContext(pluginLocation);
        return AssemblyLoadContext.LoadFromAssemblyName(
            new AssemblyName(Path.GetFileNameWithoutExtension(pluginLocation)));
    }
}