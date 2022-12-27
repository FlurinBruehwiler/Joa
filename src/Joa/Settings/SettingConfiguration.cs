using System.Reflection;

namespace Joa.Settings;

public class SettingConfiguration
{
    public SettingConfiguration(SettingOption settingOption)
    {
        PropertyDefinitions = settingOption.Value.GetType().GetProperties().Select<PropertyInfo, ISettingsProperty>(x =>
        {
            if (x.PropertyType.IsAssignableTo(typeof(List<>)))
            {
                return new SettingsPropertyList(settingOption.Value, x);
            }

            if (x.PropertyType.IsPrimitive || x.PropertyType == typeof(decimal) || x.PropertyType == typeof(string))
            {
                return new SettingsProperty(settingOption.Value, x);
            }

            throw new Exception($"The type {x.PropertyType.Name} is not allowed");
        }).ToList();
    }

    public List<ISettingsProperty> PropertyDefinitions { get; set; }
}