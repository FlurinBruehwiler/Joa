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
            var plugins = pluginLoader.GetPlugins();
        }
    }
}