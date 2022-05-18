using System.Dynamic;
using System.Reflection;
using Interfaces.Settings;

namespace JoaCore;

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
            var newValue = value;
            if(value is List<SettingsCollection> list)
            {
                //ToDo convert List<SettingsCollection> to List<"Type">
                var listType = Value.GetType().GetGenericArguments().First() ?? throw new Exception();
                var newList = new List<object>();
                foreach (var settingsCollection in list)
                {
                    var newListItem = Activator.CreateInstance(listType) ?? throw new Exception();
                    newList.Add(newListItem);
                }
                newValue = newList;
            }
            _propertyInfo.SetValue(_plugin, newValue);
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