using System.Collections;
using System.Reflection;
using System.Text.Json.Serialization;
using Interfaces;
using Interfaces.Settings;

namespace JoaCore;

public class PluginSetting
{
    public SettingPropertyAttribute SettingInfo { get; set; }

    public object Value
    {
        get
        {
            if (_list != null)
            {
                return _list;
            }
            return _propertyInfo.GetValue(_plugin) ?? throw new Exception();
        }
        set
        {
            if (_list != null)
            {
                //ToDo
            }
            else
            {
                _propertyInfo.SetValue(_plugin, value);
            }
        }
    }

    public string Name { get; set; }
    
    private readonly object _plugin;
    private readonly PropertyInfo _propertyInfo;
    private readonly List<SettingsCollection>? _list;

    public PluginSetting(object plugin, PropertyInfo propertyInfo, SettingPropertyAttribute settingInfo)
    {
        _plugin = plugin;
        _propertyInfo = propertyInfo;
        SettingInfo = settingInfo;
        Name = _propertyInfo.Name;
        if (IsList(Value))
        {
            var listType = Value.GetType().GetGenericArguments().First();
            
        }
    }
    

    private bool IsList(object o)
    {
        return o is IList &&
               o.GetType().IsGenericType &&
               o.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>));
    }
}