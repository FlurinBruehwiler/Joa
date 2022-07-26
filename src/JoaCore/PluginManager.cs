using System.Reflection;
using JoaCore.PluginCore;
using JoaCore.Settings;
using JoaPluginsPackage.Logger;
using JoaPluginsPackage.Plugin;
using JoaPluginsPackage.Settings.Attributes;
using Microsoft.Extensions.Configuration;

namespace JoaCore;

public class PluginManager
{
    public List<PluginDefinition<IPlugin>>? Plugins { get; set; }

    private readonly Dictionary<Type, List<PluginDefinition<IPlugin>>> _typedPluginsCache = new();
    private SettingsManager SettingsManager { get; set; }
    private readonly PluginLoader _pluginLoader;
    
    public PluginManager(SettingsManager settingsManager, IConfiguration configuration)
    {
        SettingsManager = settingsManager;
        _pluginLoader = new PluginLoader(configuration);
    }

    public List<PluginDefinition<T>> GetPluginsOfType<T>() where T : IPlugin
    {
        if (Plugins is null)
            return new List<PluginDefinition<T>>();

        if (_typedPluginsCache.TryGetValue(typeof(T), out var typedPlugins))
            return typedPlugins.Cast<PluginDefinition<T>>().ToList();

        var newTypedPlugins = Plugins.Where(x => x.Plugin.GetType().IsAssignableTo(typeof(T))).Cast<PluginDefinition<T>>().ToList();
        _typedPluginsCache.Add(typeof(T), newTypedPlugins.Cast<PluginDefinition<IPlugin>>().ToList());
        return newTypedPlugins;
    }

    public void UpdateIndexes()
    {
        if (Plugins is null)
            return;
        
        foreach (var pluginDefinition in Plugins)
        {
            try
            {
                // (pluginDefinition.Plugin as IIndexablePlugin)?.UpdateIndex();
            }
            catch (Exception e)
            {
                JoaLogger.GetInstance().Log($"There was an exception while updating the index of the plugin {pluginDefinition.PluginInfo.Name} with the following Stacktrace {e}", IJoaLogger.LogLevel.Error);
            }
        }
    }
    
    public void ReloadPlugins()
    {
        var timer = JoaLogger.GetInstance().StartMeasure();
        
        Plugins = new List<PluginDefinition<IPlugin>>();
        foreach (var plugin in _pluginLoader.InstantiatePlugins(SettingsManager.CoreSettings).ToList())
        {
            Plugins.Add(new PluginDefinition<IPlugin>(plugin, GetPluginInfos(plugin.GetType())));
        }
        SettingsManager.LoadPluginSettings(Plugins);
        UpdateIndexes();
        
        JoaLogger.GetInstance().LogMeasureResult(timer, nameof(ReloadPlugins));
    }
    
    private PluginAttribute GetPluginInfos(MemberInfo pluginType)
    {
        var attr = Attribute.GetCustomAttributes(pluginType).FirstOrDefault();

        return attr as PluginAttribute ?? new PluginAttribute(pluginType.Name, string.Empty);
    }
}