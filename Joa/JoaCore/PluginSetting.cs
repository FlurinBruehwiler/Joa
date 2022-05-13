﻿using System.Collections;
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
        get => _propertyInfo.GetValue(_plugin) ?? throw new Exception();
        set => _propertyInfo.SetValue(_plugin, value);
    }

    public string Name { get; set; }
    
    private readonly IPlugin _plugin;
    private readonly PropertyInfo _propertyInfo;
    private readonly bool _isList;

    public PluginSetting(IPlugin plugin, PropertyInfo propertyInfo, SettingPropertyAttribute settingInfo)
    {
        _plugin = plugin;
        _propertyInfo = propertyInfo;
        SettingInfo = settingInfo;
        Name = _propertyInfo.Name;
        _isList = IsList(Value);
    }
    
    public bool IsList(object o)
    {
        return o is IList &&
               o.GetType().IsGenericType &&
               o.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>));
    }
}