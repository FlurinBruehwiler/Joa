using System.Reflection;
using Interfaces;
using Interfaces.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace JoaCore;

public class PluginLoader
{
    private readonly IEnumerable<Type> _pluginTypes;

    public PluginLoader()
    {
        var assemblies = LoadAssemblies(GetPluginDllPaths());
        _pluginTypes = LoadTypes(assemblies);
    }
    
    public IEnumerable<IPlugin> InstantiatePlugins(Settings settings)
    {
        var serviceProvider = RegisterServices(settings);
        
        foreach (var type in _pluginTypes)
        {
            if (ActivatorUtilities.CreateInstance(serviceProvider, type) is not IPlugin result) continue;
            yield return result;
        }
    }

    public IEnumerable<PluginSetting> GetPluginSettings()
    {
        foreach (var type in _pluginTypes)
        {
            var propertyInfo = type.GetProperty(nameof(IPlugin.PluginSetting), BindingFlags.Public | BindingFlags.Static);

            if (propertyInfo?.GetValue(null) is not PluginSetting property)
                throw new Exception();

            yield return property;
        }
    }

    private IEnumerable<Type> LoadTypes(IEnumerable<Assembly> assemblies)
    {
        foreach (var assembly in assemblies)
        {
            yield return LoadType(assembly);
        }
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

    private IServiceProvider RegisterServices(Settings settings)
    {
        var services = new ServiceCollection();
        services.AddSingleton<ISettings>(settings);
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