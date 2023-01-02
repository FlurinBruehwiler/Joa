using Joa.PluginCore;
using Joa.Settings;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;

namespace Joa.Hubs;

public class SettingsHub : Hub
{
    private readonly JoaManager _joaManager;

    public SettingsHub(JoaManager joaManager)
    {
        _joaManager = joaManager;
    }

    public void SetProperty(Guid pluginId, string propertyName, object value)
    {
        GetRequiredService<PluginManager>()
            .GetPluginWithId(pluginId)
            .SettingConfiguration
            .GetPropertyWithName(propertyName)
            .SetValue(value);
    }

    public void SetPropertyInList(Guid pluginId, string listName, int listIndex, string propertyName, object value)
    {
        GetRequiredService<PluginManager>()
            .GetPluginWithId(pluginId)
            .SettingConfiguration
            .GetListPropertyWithNamie(listName)
            .Items[listIndex]
            .GetPropertyWithName(propertyName)
            .SetValue(value);
    }

    public void AddItemToList(Guid pluginId, string listName)
    {
        //ToDo return this somehow (in json)
        GetRequiredService<PluginManager>()
            .GetPluginWithId(pluginId)
            .SettingConfiguration
            .GetListPropertyWithNamie(listName)
            .AddItem();
    }

    public void GetAllSettings()
    {
        var setting = new DtoSetting();

        foreach (var pluginDefinition in GetRequiredService<PluginManager>()
                     .Plugins)
        {
            var templates = new Dictionary<PropertyDescription, DtoTemplate>();

            var plugin = new DtoPlugin
            {
                Id = pluginDefinition.Id,
                Name = "not shure",
                Description = "desc",
            };

            plugin.Fields = AddClassInstance(pluginDefinition.SettingConfiguration, plugin, templates).ToList();


            setting.Plugins.Add(plugin);
        }
    }

    private IEnumerable<DtoField> AddClassInstance(ClassInstance classInstance, DtoPlugin plugin,
        Dictionary<PropertyDescription, DtoTemplate> templates)
    {
        foreach (var property in classInstance.PropertyInstances)
        {
            if (!templates.ContainsKey(property.PropertyDescription))
            {
                templates.Add(property.PropertyDescription, new DtoTemplate
                {
                    Name = property.PropertyDescription.PropertyInfo.Name
                });
            }

            var field = new DtoField
            {
                TemplateId = property.PropertyDescription.Id,
                Value = GetValue(property, plugin, templates)
            };

            yield return field;
        }
    }

    private object GetValue(PropertyInstance property, DtoPlugin plugin,
        Dictionary<PropertyDescription, DtoTemplate> templates)
    {
        if (property is not ListPropertyInstance listProperty)
            return property.GetValue();

        var items = new List<List<DtoField>>();

        foreach (var item in listProperty.Items)
        {
            items.Add(AddClassInstance(item, plugin, templates).ToList());
        }

        return items;
    }

    private T GetRequiredService<T>()
    {
        return (T)_joaManager.CurrentScope?.ServiceProvider.GetRequiredService(typeof(T))! ??
               throw new InvalidOperationException();
    }
}