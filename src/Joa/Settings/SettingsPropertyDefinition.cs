using System.Collections;
using System.Reflection;
using JoaLauncher.Api;

namespace Joa.Settings;

public interface ISettingsProperty
{
}

public class SettingsProperty : ISettingsProperty
{
    public PropertyInfo PropertyInfo { get; set; }

    private readonly ISetting _setting;
    
    public SettingsProperty(ISetting setting, PropertyInfo propertyInfo)
    {
        _setting = setting;
        PropertyInfo = propertyInfo;
    }
    
    public void SetValue(object value)
    {
        PropertyInfo.SetValue(_setting, value);
    }

    public object GetValue()
    {
        return PropertyInfo.GetValue(_setting) ?? throw new Exception("Could not get value");
    }
}

public class SettingsPropertyList : ISettingsProperty
{
    public PropertyInfo PropertyInfo { get; set; }
    
    private readonly Type _genericType;
    private readonly MethodInfo _addMethod;
    private readonly ISetting _setting;
    public List<PropertyInfo> SettingsProperties { get; set; }

    public SettingsPropertyList(ISetting setting, PropertyInfo propertyInfo)
    {
        _setting = setting;
        PropertyInfo = propertyInfo;
        _genericType = PropertyInfo.PropertyType.GenericTypeArguments.First();

        SettingsProperties = _genericType.GetProperties().ToList();

        _addMethod = PropertyInfo.PropertyType.GetMethod("Add")!;
    }
    
    public object AddItem()
    {
        var newItem = Activator.CreateInstance(_genericType) ?? throw new Exception("Could not create Instance");
        _addMethod.Invoke(PropertyInfo.GetValue(_setting), new[] { newItem });
        return newItem;
    }

    public void SetPropertyOnItem(int listIndex, string propertyName, object value)
    {
        var item = ((IList)PropertyInfo.GetValue(_setting)!)[listIndex];
        var settingsProperty = SettingsProperties.First(x => x.Name == propertyName);
        settingsProperty.SetValue(item, value);
    }

    public IList GetValues()
    {
        return (IList)PropertyInfo.GetValue(_setting)!;
    }
}
