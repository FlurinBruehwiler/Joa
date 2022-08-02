using System.Reflection;
using JoaCore.PluginCore;
using JoaCore.Settings;
using JoaPluginsPackage.Attributes;
using JoaPluginsPackage.Injectables;
using JoaPluginsPackage.Plugin;
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
        return GetPluginDefinitionsOfType<T>().Select(x => (T)x.Plugin).ToList();
    }

    public List<PluginDefinition> GetPluginDefinitionsOfType<T>() where T : IPlugin
    {
        if (Plugins is null)
            return new List<PluginDefinition>();

        return Plugins.Where(x => x.Plugin is T).ToList();
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
                JoaLogger.GetInstance().Log($"There was an exception while updating the index of the plugin {plugin.GetType().Name} with the following Stacktrace {e}", IJoaLogger.LogLevel.Error);
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
        if (Attribute.GetCustomAttributes(pluginType).FirstOrDefault(x => x is PluginAttribute) is PluginAttribute pluginAttribute)
            return pluginAttribute;
        
        throw new Exception($"The plugin {pluginType.Name} does not have the PluginAttribute");
    }
}