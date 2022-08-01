using JoaPluginsPackage.Plugin;

namespace JoaCore;

public static class PluginExtensions
{
    public static T GetTypedPlugin<T>(this IPlugin plugin)
    {
        return (T)plugin;
    }
}