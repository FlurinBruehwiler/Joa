using System.Reflection;
using Interfaces;
using Interfaces.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace AppWithPlugin;

public class PluginLoader
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IEnumerable<Assembly> _assemblies;

    public PluginLoader(Settings settings)
    {
        _serviceProvider = RegisterServices(settings);
        _assemblies = LoadAssemblys(GetPluginDllPaths());
    }

    private IServiceProvider RegisterServices(Settings settings)
    {
        var services = new ServiceCollection();
        services.AddSingleton<ISettings>(settings);
        return services.BuildServiceProvider();
    }

    public IEnumerable<IPlugin> GetPlugins()
    {
        return _assemblies.Select(InstantiatePlugin);
    }

    private IEnumerable<string> GetPluginDllPaths()
    {
        var pluginFolder = Path.GetFullPath(Path.Combine(typeof(PluginLoader).Assembly.Location, @"..\..\..\..\..\Plugins"));
        
        Console.WriteLine($"Searching for Plugins in {pluginFolder}");
        
        var pluginFolders = Directory.GetDirectories(pluginFolder);

        var plugins = pluginFolders.Select(x => Directory.GetFiles(x)
            .FirstOrDefault(file => file.EndsWith($"{Directory.GetParent(file)?.Name}.dll"))).Where(x => x != null).ToList();
        
        Console.WriteLine($"Found the following {plugins.Count} plugins: ");
        foreach (var plugin in plugins)
        {
            Console.WriteLine(plugin);
        }

        return plugins!;
    }

    private IEnumerable<Assembly> LoadAssemblys(IEnumerable<string> dllPaths)
    {
        return dllPaths.Select(LoadAssembly);
    }

    private Assembly LoadAssembly(string relativePluginPath)
    {
        var root = Path.Combine(typeof(PluginLoader).Assembly.Location, @"..\..\..\..\..\");

        var pluginLocation = Path.GetFullPath(Path.Combine(root, relativePluginPath.Replace('\\', Path.DirectorySeparatorChar)));
        var loadContext = new PluginLoadContext(pluginLocation);
        return loadContext.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(pluginLocation)));
    }

    private IPlugin InstantiatePlugin(Assembly assembly)
    {
        foreach (var type in assembly.GetTypes())
        {
            if (!typeof(IPlugin).IsAssignableFrom(type)) continue;
            if (ActivatorUtilities.CreateInstance(_serviceProvider, type) is not IPlugin result) continue;
            return result;
        }

        var availableTypes = string.Join(",", assembly.GetTypes().Select(t => t.FullName));
        throw new ApplicationException(
            $"Can't find any type which implements IPlugin in {assembly} from {assembly.Location}.\n" +
            $"Available types: {availableTypes}");
    }
}