using System.Reflection;
using Interfaces.Plugin;
using Interfaces.Settings.Attributes;
using JoaCore.PluginCore;
using JoaCore.Settings;
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
    
    public void UpdateIndexes()
    {
        if (Plugins is null)
            return;

        foreach (var pluginDefinition in Plugins)
        {
            (pluginDefinition.Plugin as IIndexablePlugin)?.UpdateIndex();
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