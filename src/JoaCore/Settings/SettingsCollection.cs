using System.Reflection;
using JoaPluginsPackage.Attributes;

namespace JoaCore.Settings;

public class SettingsCollection
{
    public List<PluginSetting> PluginSettings { get; set; }
    
    public SettingsCollection(object objectWhereTheSettingAreOn)
    {
        PluginSettings = GetPluginSettings(objectWhereTheSettingAreOn).ToList();
    }

    private IEnumerable<PluginSetting> GetPluginSettings(object plugin)
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