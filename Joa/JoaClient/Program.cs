using PluginBase;
using System.Reflection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace AppWithPlugin
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var pluginLoader = new PluginLoader();
            
            try
            {
                var plugins = pluginLoader.GetPluginDllPaths();
                
                IEnumerable<IPlugin> commands = plugins.SelectMany(pluginPath =>
                {
                    Assembly pluginAssembly = pluginLoader.LoadPlugin(pluginPath);
                    return pluginLoader.CreatePlugins(pluginAssembly);
                }).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}