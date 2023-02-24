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
    public required Type GenericType { get; set; }
    public required MethodInfo AddMethod { get; set; }
}

public class ClassDescription
{
    public ClassDescription(Type type)
    {
        PropertyDescriptions = type.GetProperties().Select(x => new PropertyDescription
        {
            PropertyInfo = x
        }).ToList();
    }

    public List<PropertyDescription> PropertyDescriptions { get; }
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

    public List<ClassInstance> Items { get; } = new();

    public ListPropertyInstance(object instance, ListPropertyDescription propertyDescription) : base(instance, propertyDescription)
    {
        _instance = instance;
        _propertyDescription = propertyDescription;

        var list = GetListValue();
        var classDescription = new ClassDescription(list.GetType().GetGenericArguments().First());

        foreach (var item in list)
        {
            Items.Add(new ClassInstance(item, classDescription));
        }
    }

    public IList GetListValue()
    {
        return (IList)GetValue();
    }

    public object AddItem()
    {
        var newItem = Activator.CreateInstance(_propertyDescription.GenericType) ?? throw new Exception("Could not create Instance");
        _propertyDescription.AddMethod.Invoke(_propertyDescription.PropertyInfo.GetValue(_instance), new[] { newItem });
        return newItem;
    }
}

public class ClassInstance
{
    public ClassDescription ClassDescription { get; }
    public List<PropertyInstance> PropertyInstances { get; } = new();

    public ClassInstance(object instance, ClassDescription classDescription)
    {
        ClassDescription = classDescription;

        var addMethod = typeof(List<>).GetMethod("Add");

        if (addMethod is null)
            throw new UnreachableException();

        foreach (var propertyInfo in instance.GetType().GetProperties())
        {
            var type = propertyInfo.PropertyType;
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                var pd = new ListPropertyDescription
                {
                    PropertyInfo = propertyInfo,
                    AddMethod = addMethod,
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