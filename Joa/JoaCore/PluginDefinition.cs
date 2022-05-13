using System.Reflection;
using Interfaces;
using Interfaces.Settings;

namespace JoaCore;

public class PluginDefinition
{
    public IEnumerable<PluginSetting> PluginSettings { get; set; }
    public string Name { get; set; }
    
    public PluginDefinition(IPlugin plugin)
    {
        PluginSettings = GetPluginSettings(plugin);
        Name = plugin.GetType().Name;
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

            if (attr is not SettingPropertyAttribute settingPropertyAttribute)
                continue;

            yield return (propertyInfo, settingPropertyAttribute);
        }
    }
}