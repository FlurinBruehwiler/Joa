using System.Reflection;
using Interfaces;
using Interfaces.Settings;

namespace JoaCore;

public class PluginDefinition
{
    public IEnumerable<PluginSetting> PluginSettings { get; set; }
    
    public PluginDefinition(IPlugin plugin)
    {
        PluginSettings = GetPluginSettings(plugin);
    }

    private IEnumerable<PluginSetting> GetPluginSettings(IPlugin plugin)
    {
        foreach (var (propertyInfo, settingPropertyAttribute) in GetPropertyInfos(plugin.GetType()))
        {
            yield return new PluginSetting(plugin, propertyInfo, settingPropertyAttribute);
        }
    }

    private IEnumerable<(PropertyInfo, SettingPropertyAttribute)> GetPropertyInfos(Type pluginType)
    {
        foreach (var propertyInfo in pluginType.GetProperties())
        {
            var attr = Attribute.GetCustomAttribute(propertyInfo, typeof(SettingPropertyAttribute));
            if(attr == null)
                continue;

            if (attr is not SettingPropertyAttribute settingPropertyAttribute)
                continue;
            
            yield return (propertyInfo, settingPropertyAttribute);
        }
    }
}