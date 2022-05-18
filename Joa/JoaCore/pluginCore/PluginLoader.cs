using System.Reflection;
using Interfaces;
using Interfaces.Logger;
using Interfaces.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace JoaCore.pluginCore;

public class PluginLoader
{
    private readonly IEnumerable<Type> _pluginTypes;

    public PluginLoader()
    {
        var assemblies = LoadAssemblies(GetPluginDllPaths());
        _pluginTypes = LoadTypes(assemblies);
    }
    
    public IEnumerable<IPlugin> InstantiatePlugins(CoreSettings coreSettings)
    {
        var serviceProvider = RegisterServices(coreSettings);
        
        foreach (var type in _pluginTypes)
        {
            if (ActivatorUtilities.CreateInstance(serviceProvider, type) is not IPlugin result) continue;
            yield return result;
        }
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

    private IEnumerable<Type> LoadTypes(IEnumerable<Assembly> assemblies)
    {
        return assemblies.Select(assembly => LoadType(assembly));
    }

    private Type LoadType(Assembly assembly)
    {
        foreach (var type in assembly.GetTypes())
        {
            if (!typeof(IPlugin).IsAssignableFrom(type)) continue;
            return type;
        }
        
        var availableTypes = string.Join(",", assembly.GetTypes().Select(t => t.FullName));
        throw new ApplicationException(
            $"Can't find any type which implements IPlugin in {assembly} from {assembly.Location}.\n" +
            $"Available types: {availableTypes}");
    }

    private IServiceProvider RegisterServices(CoreSettings coreSettings)
    {
        var services = new ServiceCollection();
        services.AddSingleton<IJoaSettings>(coreSettings);
        services.AddSingleton<IJoaLogger>(LoggingManager.JoaLogger);
        return services.BuildServiceProvider();
    }
    
    private IEnumerable<string> GetPluginDllPaths()
    {
        var pluginFolder =
            Path.GetFullPath(Path.Combine(typeof(PluginLoader).Assembly.Location, @"..\..\..\..\..\Plugins"));

        Console.WriteLine($"Searching for Plugins in {pluginFolder}");

        var pluginFolders = Directory.GetDirectories(pluginFolder);

        var plugins = pluginFolders.Select(x => Directory.GetFiles(x)
                .FirstOrDefault(file => file.EndsWith($"{Directory.GetParent(file)?.Name}.dll"))).Where(x => x != null)
            .ToList();

        Console.WriteLine($"Found the following {plugins.Count} plugins: ");
        foreach (var plugin in plugins)
        {
            Console.WriteLine(plugin);
        }

        return plugins!;
    }

    private IEnumerable<Assembly> LoadAssemblies(IEnumerable<string> dllPaths)
    {
        return dllPaths.Select(LoadAssembly);
    }

    private Assembly LoadAssembly(string relativePluginPath)
    {
        var root = Path.Combine(typeof(PluginLoader).Assembly.Location, @"..\..\..\..\..\");

        var pluginLocation =
            Path.GetFullPath(Path.Combine(root, relativePluginPath.Replace('\\', Path.DirectorySeparatorChar)));
        var loadContext = new PluginLoadContext(pluginLocation);
        return loadContext.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(pluginLocation)));
    }

}