using System.Reflection;
using JoaCore.PluginCore;
using JoaCore.Settings;
using JoaPluginsPackage.Logger;
using JoaPluginsPackage.Plugin;
using JoaPluginsPackage.Plugin.Search;
using JoaPluginsPackage.Settings.Attributes;
using Microsoft.Extensions.Configuration;

namespace JoaCore;

public class PluginManager
{
    public List<PluginDefinition>? Plugins { get; set; }
    private SettingsManager SettingsManager { get; set; }
    private readonly PluginLoader _pluginLoader;
    
    public PluginManager(SettingsManager settingsManager, IConfiguration configuration)
    {
        SettingsManager = settingsManager;
        _pluginLoader = new PluginLoader(configuration);
    }

    public List<T> GetPluginsOfType<T>() where T : IPlugin
    {
        if (Plugins is null)
            return new List<T>();

        return Plugins.Where(x => x.Plugin is T).Select(x => (T)x.Plugin).ToList();
    }

    public void UpdateIndexes()
    {
        foreach (var plugin in GetPluginsOfType<IGlobalSearchPlugin>())
        {
            try
            {
                plugin.UpdateIndex();           
            }
            catch (Exception e)
            {
                JoaLogger.GetInstance().Log($"There was an exception while updating the index of the plugin {plugin.Name} with the following Stacktrace {e}", IJoaLogger.LogLevel.Error);
            }
        }
    }
    
    public void ReloadPlugins()
    {
        var timer = JoaLogger.GetInstance().StartMeasure();
        
        Plugins = new List<PluginDefinition>();
        foreach (var plugin in _pluginLoader.InstantiatePlugins(SettingsManager.CoreSettings).ToList())
        {
            Plugins.Add(new PluginDefinition(plugin, GetPluginInfos(plugin.GetType())));
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