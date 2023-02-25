using System.Collections;
using System.Diagnostics;
using System.Reflection;

namespace Joa.Settings;

public class PropertyDescription
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required PropertyInfo PropertyInfo { get; set; }
}

public class ListPropertyDescription : PropertyDescription
{
    public Type Type { get; set; }
    public required Type GenericType { get; set; }
    public MethodInfo AddMethod { get; set; }
    public MethodInfo RemoveMethod { get; set; }

    public ListPropertyDescription(Type type)
    {
        Type = type;
        AddMethod = Type.GetMethod("Add");
        RemoveMethod = Type.GetMethod("Remove");
    }
}

public class ClassDescription
{
    private readonly Type _type;
    
    public ClassDescription(Type type)
    {
        _type = type;
        PropertyDescriptions = type.GetProperties().Select(x => new PropertyDescription
        {
            PropertyInfo = x
        }).ToList();
    }

    public List<PropertyDescription> PropertyDescriptions { get; }

    public object CreateInstance()
    {
        return Activator.CreateInstance(_type) ?? throw new Exception("Could not create Instance");
    }
}

public class PropertyInstance
{
    private object _instance;

    public PropertyInstance(object instance, PropertyDescription propertyDescription)
    {
        _instance = instance;
        PropertyDescription = propertyDescription;
    }

    public PropertyDescription PropertyDescription { get; }

    public void SetValue(object value)
    {
        PropertyDescription.PropertyInfo.SetValue(_instance, value);
    }

    public object GetValue()
    {
        return PropertyDescription.PropertyInfo.GetValue(_instance) ?? throw new Exception("Could not get value");
    }
}

public class ListPropertyInstance : PropertyInstance
{
    private readonly object _instance;
    private readonly ListPropertyDescription _propertyDescription;
    public ClassDescription ClassDescription { get; }

    public ListPropertyInstance(object instance, ListPropertyDescription propertyDescription) : base(instance, propertyDescription)
    {
        _instance = instance;
        _propertyDescription = propertyDescription;
        ClassDescription = new ClassDescription(((IList)GetValue()).GetType().GetGenericArguments().First());
    }

    public List<ClassInstance> GetItems()
    {
        return ((IList)GetValue()).Cast<object>().Select(x => new ClassInstance(x, ClassDescription)).ToList();
    }
    
    public void AddItem(object item)
    {
        _propertyDescription.AddMethod.Invoke(_propertyDescription.PropertyInfo.GetValue(_instance), new[] { item });
    }
    
    public void RemoveItem(ClassInstance item)
    {
        _propertyDescription.RemoveMethod.Invoke(_propertyDescription.PropertyInfo.GetValue(_instance), new[] { item.Instance });
    }
}

public class ClassInstance
{
    public ClassDescription ClassDescription { get; }
    public List<PropertyInstance> PropertyInstances { get; } = new();

    public object Instance { get; }

    public ClassInstance(object instance, ClassDescription classDescription)
    {
        ClassDescription = classDescription;
        Instance = instance;
        
        foreach (var propertyInfo in instance.GetType().GetProperties())
        {
            var type = propertyInfo.PropertyType;
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                var pd = new ListPropertyDescription(type)
                {
                    PropertyInfo = propertyInfo,
                    GenericType = propertyInfo.PropertyType.GetGenericArguments().First()
                };
                ClassDescription.PropertyDescriptions.Add(pd);
                PropertyInstances.Add(new ListPropertyInstance(instance, pd));
            }

            if (type.IsPrimitive || propertyInfo.PropertyType == typeof(decimal) || propertyInfo.PropertyType == typeof(string))
            {
                var pd = new PropertyDescription
                {
                    PropertyInfo = propertyInfo
                };
                ClassDescription.PropertyDescriptions.Add(pd);
                PropertyInstances.Add(new PropertyInstance(instance, pd));
            }
        }
    }

    public PropertyInstance GetPropertyWithName(string name)
    {
        return PropertyInstances
            .First(x => x.PropertyDescription.PropertyInfo.Name == name);
    }

    public ListPropertyInstance GetListPropertyWithNamie(string name)
    {
        return (ListPropertyInstance)GetPropertyWithName(name);
    }
}