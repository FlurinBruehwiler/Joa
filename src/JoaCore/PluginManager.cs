using System.Reflection;
using JoaCore.PluginCore;
using JoaCore.Settings;
using JoaPluginsPackage.Attributes;
using JoaPluginsPackage.Injectables;
using JoaPluginsPackage.Plugin;

namespace JoaCore;

public class PluginManager
{
    public List<PluginDefinition>? Plugins { get; set; }
    private SettingsManager SettingsManager { get; set; }
    private readonly PluginLoader _pluginLoader;
    private readonly IJoaLogger _logger;
    private readonly IconManager _iconManager;

    public PluginManager(SettingsManager settingsManager, PluginLoader pluginLoader, IJoaLogger logger, IconManager iconManager)
    {
        SettingsManager = settingsManager;
        _pluginLoader = pluginLoader;
        _logger = logger;
        _iconManager = iconManager;
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
                _iconManager.UpdateIcons(Plugins);
            }
            catch (Exception e)
            {
                _logger.LogException(e, $"Updating the index for plugin {plugin.GetType().Name} failed");
            }
        }
    }

    public void ReloadPlugins()
    {
        var timer = _logger.StartMeasure();
    
        Plugins = new List<PluginDefinition>();
        foreach (var plugin in _pluginLoader.InstantiatePlugins().ToList())
        {
            var pluginInfos = GetPluginInfos(plugin.GetType());
            if(pluginInfos is null)
                continue;
            Plugins.Add(new PluginDefinition(plugin, pluginInfos));
        }
        SettingsManager.LoadPluginSettings(Plugins);
        UpdateIndexes();
    
        _logger.LogMeasureResult(timer, nameof(ReloadPlugins));
    }
    
    private PluginAttribute? GetPluginInfos(MemberInfo pluginType)
    {
        if (Attribute.GetCustomAttributes(pluginType).FirstOrDefault(x => x is PluginAttribute) is PluginAttribute pluginAttribute)
            return pluginAttribute;
        
        
        _logger.Log($"The plugin {pluginType.Name} does not have the PluginAttribute", IJoaLogger.LogLevel.Error);
        return null;
    }
}