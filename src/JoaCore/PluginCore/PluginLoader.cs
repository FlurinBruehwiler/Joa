using System.Reflection;
using System.Text;
using JoaCore.Settings;
using JoaPluginsPackage.Logger;
using JoaPluginsPackage.Plugin;
using JoaPluginsPackage.Settings;
using JoaPluginsPackage.Settings.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace JoaCore.PluginCore;

public class PluginLoader
{
    private readonly List<Type> _pluginTypes;

    public PluginLoader(IConfiguration configuration)
    {
        var assemblies = LoadAssemblies(GetPluginDllPaths(configuration));
        _pluginTypes = LoadTypes(assemblies);
    }
    
    public IEnumerable<IPlugin> InstantiatePlugins(CoreSettings coreSettings)
    {
        var output = new List<IPlugin>();

        var serviceProvider = RegisterServices(coreSettings);
        
        foreach (var type in _pluginTypes)
        {
            if (ActivatorUtilities.CreateInstance(serviceProvider, type) is not IPlugin result) continue;
            output.Add(result);
        }
        
        JoaLogger.GetInstance().Log("Loaded Plugins successfully!!", IJoaLogger.LogLevel.Info);
        
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
        JoaLogger.GetInstance().Log($"Can't find any type which implements IPlugin in {assembly} from {assembly.Location}.\n" +
                                     $"Available types: {availableTypes}", IJoaLogger.LogLevel.Warning);
        return null;
    }

    private IServiceProvider RegisterServices(CoreSettings coreSettings)
    {
        var services = new ServiceCollection();
        services.AddSingleton<IJoaSettings>(coreSettings);
        services.AddSingleton<IJoaLogger>(JoaLogger.GetInstance());
        return services.BuildServiceProvider();
    }
    
    private List<string> GetPluginDllPaths(IConfiguration configuration)
    {
        var path = configuration.GetValue<string>("PluginLocation");
        
        var pluginFolder =
            Path.GetFullPath(Path.Combine(typeof(PluginLoader).Assembly.Location, path));

        JoaLogger.GetInstance().Log($"Searching for Plugins in {pluginFolder}", IJoaLogger.LogLevel.Info);

        var pluginFolders = Directory.GetDirectories(pluginFolder);

        var plugins = pluginFolders.Select(x => Directory.GetFiles(x)
                .FirstOrDefault(file => file.EndsWith($"{Directory.GetParent(file)?.Name}.dll"))).Where(x => x != null)
            .ToList();

        var pluginsToLog = plugins.Aggregate("", (current, plugin) => current + Environment.NewLine + plugin);
        JoaLogger.GetInstance().Log($"Found the following plugins DLLs: {pluginsToLog}", IJoaLogger.LogLevel.Info);

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