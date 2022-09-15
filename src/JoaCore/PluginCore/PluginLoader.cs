using System.Reflection;
using JoaCore.Settings;
using JoaPluginsPackage.Attributes;
using JoaPluginsPackage.Injectables;
using JoaPluginsPackage.Plugin;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace JoaCore.PluginCore;

public class PluginLoader
{
    private readonly List<Type> _pluginTypes;
    private readonly IJoaLogger _logger;
    private readonly ServiceProviderForPlugins _serviceProvider;

    public PluginLoader(IConfiguration configuration, IJoaLogger logger, ServiceProviderForPlugins serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        var assemblies = LoadAssemblies(GetPluginDllPaths(configuration));
        _pluginTypes = LoadTypes(assemblies);
    }
    
    public IEnumerable<IPlugin> InstantiatePlugins()
    {
        var output = new List<IPlugin>();

        foreach (var type in _pluginTypes)
        {
            if (ActivatorUtilities.CreateInstance(_serviceProvider.ServiceProvider, type) is not IPlugin result) continue;
            output.Add(result);
        }
        
        _logger.Log("Loaded Plugins successfully!!", IJoaLogger.LogLevel.Info);
        
        return output;
    }
    
    private IEnumerable<PropertyInfo> GetSettingsForPlugin(Type pluginType)
    {
        foreach (var propertyInfo in pluginType.GetProperties())
        {
            var attr = Attribute.GetCustomAttribute(propertyInfo, typeof(SettingPropertyAttribute));
            if(attr is not SettingPropertyAttribute settingPropertyAttribute) continue;
            yield return propertyInfo;
        }
    }

    private List<Type> LoadTypes(List<Assembly> assemblies)
    {
        return assemblies.Select(LoadType).Where(type => type != null).ToList()!;
    }

    private Type? LoadType(Assembly assembly)
    {
        foreach (var type in assembly.GetTypes())
        {
            if (!typeof(IPlugin).IsAssignableFrom(type)) continue;
            return type;
        }
        
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