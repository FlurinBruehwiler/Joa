using System.Collections;
using System.Reflection;
using JoaPluginsPackage.Attributes;

namespace JoaCore.Settings;

public class PluginSetting
{
    public SettingPropertyAttribute SettingInfo { get; set; }

    public object Value
    {
        get
        {
            var value = _propertyInfo.GetValue(_plugin) ?? throw new Exception();
            return value is not List<object> list ? value : list.Select(o => new SettingsCollection(o)).ToList();
        }
        set
        {
            if(value is List<Dictionary<string, object>> list)
            {
                var listType = Value.GetType().GetGenericArguments().First() ?? throw new Exception();
                var newList = Activator.CreateInstance(typeof(List<>).MakeGenericType(listType)) as IList;
                foreach (var settingsCollection in list)
                {
                    var newListItem = Activator.CreateInstance(listType) ?? throw new Exception();
                    foreach (var propertyInfo in newListItem.GetType().GetProperties())
                    {
                        propertyInfo.SetValue(newListItem, settingsCollection[propertyInfo.Name]);
                    }
                    newList?.Add(newListItem);
                }
                _propertyInfo.SetValue(_plugin, newList);
                return;
            }
            _propertyInfo.SetValue(_plugin, value);
        }
    }

    public string Name { get; set; }
    
    private readonly object _plugin;
    private readonly PropertyInfo _propertyInfo;

    public PluginSetting(object plugin, PropertyInfo propertyInfo, SettingPropertyAttribute settingInfo)
    {
        _plugin = plugin;
        _propertyInfo = propertyInfo;
        SettingInfo = settingInfo;
        Name = _propertyInfo.Name;
    }
}