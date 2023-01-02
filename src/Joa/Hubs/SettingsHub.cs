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
            var plugin = new DtoPlugin
            {
                Id = pluginDefinition.Id,
                Name = "not shure",
                Description = "desc",
            };
            
            plugin.Fields = AddClassInstance(pluginDefinition.SettingConfiguration, plugin).ToList();

            
            setting.Plugins.Add(plugin);
        }
    }

    private IEnumerable<DtoField> AddClassInstance(ClassInstance classInstance, DtoPlugin plugin)
    {
        foreach (var property in classInstance.PropertyInstances)
        {
            var template = new DtoTemplate
            {
                Name = property.PropertyDescription.PropertyInfo.Name
            };
            var templateId = 

            var field = new DtoField
            {
                TemplateId = templateId,
                Value = GetValue(property, plugin)
            };

            plugin.Templates.Add(templateId, template);
            yield return field;
        }
    }

    private object GetValue(PropertyInstance property, DtoPlugin plugin)
    {
        if (property is not ListPropertyInstance listProperty)
            return property.GetValue();

        var items = new List<List<DtoField>>();

        foreach (var item in listProperty.Items)
        {
            items.Add(AddClassInstance(item, plugin).ToList());
        }

        return items;
    }

    private T GetRequiredService<T>()
    {
        return (T)_joaManager.CurrentScope?.ServiceProvider.GetRequiredService(typeof(T))! ?? throw new InvalidOperationException();
    }
}