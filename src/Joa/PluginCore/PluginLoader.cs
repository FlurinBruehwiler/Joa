using System.Reflection;
using System.Runtime.Loader;
using System.Text.Json;
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
            pluginDefinitions.Add(pluginBuilder.BuildPluginDefinition(plugin, setting, caches, asyncCaches, pluginType.PluginManifest));
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

    private List<PluginTypes> LoadTypes(List<PluginAssembly> assemblies)
    {
        return assemblies.Select(LoadType).Where(type => type is not null).ToList()!;
    }

    private PluginTypes? LoadType(PluginAssembly pluginAssembly)
    {
        var assembly = pluginAssembly.Assembly;
        
        PluginTypes pluginTypes = new()
        {
            PluginManifest = pluginAssembly.PluginManifest
        };

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
    
    private List<PluginFiles> GetPluginDllPaths(string pluginsFinalLocation)
    {
        _logger.Log($"Searching for Plugins in {pluginsFinalLocation}", LogLevel.Info);

        var pluginFolders = Directory.GetDirectories(pluginsFinalLocation);
        
        var plugins = pluginFolders.Select(pluginfolder =>
            {
                var filesInFolder = Directory.GetFiles(pluginfolder);

                var manifest = filesInFolder.FirstOrDefault(file => file == "manifest.json");

                var dll = filesInFolder.FirstOrDefault(file => file.EndsWith($"{Directory.GetParent(file)?.Name}.dll"));

                if (dll is null)
                    return null;

                return new PluginFiles(dll, manifest);
            }).Where(x => x is not null)
            .ToList();

        var pluginsToLog = plugins.Aggregate("", (current, plugin) => current + Environment.NewLine + plugin);
        _logger.Log($"Found the following plugins DLLs: {pluginsToLog}", LogLevel.Info);

        return plugins!;
    }
    

    private List<PluginAssembly> LoadAssemblies(List<PluginFiles> pluginFiles)
    {
        return pluginFiles.Select(s =>
        {
            var assembly = LoadAssembly(s.Dll);
            if (s.Manifest is not null)
            {
                var manifestString = File.ReadAllText(s.Manifest);
                try
                {
                    var manifest = JsonSerializer.Deserialize<PluginManifest>(manifestString);
                    if (manifest is null)
                        throw new Exception();
                    
                    return new PluginAssembly(assembly, manifest);
                }
                catch
                {
                    _logger.Error($"Error parsing the manifest file '{s.Manifest}'");
                }
            }

            var assemblyName = assembly.GetName().Name;

            if (assemblyName is null)
                throw new Exception($"Could not read assembly name from name from {s.Dll}");
            
            var beautifiedName = BeautifyString(assemblyName);
            var generatedManifest = new PluginManifest
            {
                Id = assemblyName,
                Name = beautifiedName
            };

            return new PluginAssembly(assembly, generatedManifest);
        }).ToList();
    }

    private Assembly LoadAssembly(string pluginLocation)
    {
        AssemblyLoadContext = new PluginLoadContext(pluginLocation);
        return AssemblyLoadContext.LoadFromAssemblyName(
            new AssemblyName(Path.GetFileNameWithoutExtension(pluginLocation)));
    }

    private string BeautifyString(string inputString)
    {
        var outputString = string.Empty;

        for (var i = 0; i < inputString.Length; i++)
        {
            if (i > 0 && char.IsUpper(inputString[i]))
            {
                outputString += " ";
            }
            outputString += inputString[i];
        }

        return outputString;
    }
}


record PluginFiles(string Dll, string? Manifest);

record PluginAssembly(Assembly Assembly, PluginManifest PluginManifest);