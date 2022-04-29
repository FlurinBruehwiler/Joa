using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using PluginBase;

namespace AppWithPlugin;

public class PluginLoader
{
    private IServiceProvider ServiceProvider { get; set; }

    public PluginLoader()
    {
        ServiceProvider = RegisterServices();
    }

    private IServiceProvider RegisterServices()
    {
        var services = new ServiceCollection();
        services.AddSingleton<ISettings, Settings>();
        return services.BuildServiceProvider();
    }

    public List<IPlugin> GetPlugins()
    {
        List<IPlugin> plugins = new(); 

        try
        {
            var pluginDllPaths = GetPluginDllPaths();
                
            plugins = pluginDllPaths.Select(pluginPath =>
            {
                Assembly pluginAssembly = LoadPlugin(pluginPath);
                return CreatePlugin(pluginAssembly);
            }).ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }

        return plugins;
    }

    private IEnumerable<string> GetPluginDllPaths()
    {
        var pluginFolder = Path.GetFullPath(Path.Combine(typeof(Program).Assembly.Location, @"..\..\..\..\..\Plugins"));
        
        Console.WriteLine($"Searching for Plugins in {pluginFolder}");
        
        var pluginFolders = Directory.GetDirectories(pluginFolder);

        var plugins = pluginFolders.Select(x => Directory.GetFiles(x).FirstOrDefault(file => file.EndsWith(".dll"))).Where(x => x != null).ToList();
        
        Console.WriteLine($"Found the following {plugins.Count} plugins: ");
        foreach (var plugin in plugins)
        {
            Console.WriteLine(plugin);
        }

        return plugins!;
    }

    private Assembly LoadPlugin(string relativePluginPath)
    {
        var root = Path.Combine(typeof(Program).Assembly.Location, @"..\..\..\..\..\");

        var pluginLocation = Path.GetFullPath(Path.Combine(root, relativePluginPath.Replace('\\', Path.DirectorySeparatorChar)));
        var loadContext = new PluginLoadContext(pluginLocation);
        return loadContext.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(pluginLocation)));
    }

    private IPlugin CreatePlugin(Assembly assembly)
    {
        foreach (var type in assembly.GetTypes())
        {
            if (!typeof(IPlugin).IsAssignableFrom(type)) continue;
            if (ActivatorUtilities.CreateInstance(ServiceProvider, type) is not IPlugin result) continue;
            return result;
        }

        var availableTypes = string.Join(",", assembly.GetTypes().Select(t => t.FullName));
        throw new ApplicationException(
            $"Can't find any type which implements IPlugin in {assembly} from {assembly.Location}.\n" +
            $"Available types: {availableTypes}");
    }
}