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
        GetRequiredService<PluginManager>().Plugins.First(x => x.Id == pluginId)
            .SettingConfiguration.PropertyDefinitions.OfType<SettingsProperty>()
            .First(x => x.PropertyInfo.Name == propertyName).SetValue(value);
    }
    
    public void SetPropertyInList(Guid pluginId, string listName, int listIndex, string propertyName, object value)
    {
        GetRequiredService<PluginManager>().Plugins.First(x => x.Id == pluginId)
            .SettingConfiguration.PropertyDefinitions.OfType<SettingsPropertyList>()
            .First(x => x.PropertyInfo.Name == listName)
            .SetPropertyOnItem(listIndex, propertyName, value);
    }
    
    public void AddItemToList(Guid pluginId, string listName)
    {
        //return this somehow (in json)
        var res = GetRequiredService<PluginManager>().Plugins.First(x => x.Id == pluginId)
            .SettingConfiguration.PropertyDefinitions.OfType<SettingsPropertyList>()
            .First(x => x.PropertyInfo.Name == listName)
            .AddItem();
    }

    public void GetAllSettings()
    {
        var res = new DtoFrontendSettings();
        
        foreach (var x in GetRequiredService<PluginManager>().Plugins)
        {
            foreach (var y in x.SettingConfiguration.PropertyDefinitions)
            {
                if (y is SettingsProperty settingsProperty)
                {
                    var dtoSettingsDescription = new DtoSettingsDescription
                    {
                        Name = settingsProperty.PropertyInfo.Name,
                        Attributes = settingsProperty.PropertyInfo.CustomAttributes.ToList()
                    };
                    
                    res.SettingsDescriptions.Add(dtoSettingsDescription);
                    
                    res.Properties.Add(new DtoProperty
                    {
                        Value = settingsProperty.GetValue(),
                        SettingsDescriptionId = dtoSettingsDescription.Id
                    }); 
                    
                }
                
                if (y is SettingsPropertyList settingsPropertyList)
                {
                    var dtoSettingsDescription = new DtoSettingsDescription
                    {
                        Name = settingsPropertyList.PropertyInfo.Name,
                        Attributes = settingsPropertyList.PropertyInfo.CustomAttributes.ToList()
                    };
                    
                    res.SettingsDescriptions.Add(dtoSettingsDescription);
                    
                    res.Properties.Add(new DtoProperty
                    {
                        SettingsDescriptionId = dtoSettingsDescription.Id,
                        Value = settingsPropertyList
                    }); 
                    
                    foreach (var propertyInfo in settingsPropertyList.SettingsProperties)
                    {
                        var a = new DtoSettingsDescription
                        {
                            Name = propertyInfo.Name,
                            Attributes = propertyInfo.CustomAttributes.ToList()
                        };
                    
                        res.SettingsDescriptions.Add(a);
                    }
                    
                    foreach (var value in settingsPropertyList.GetValues())
                    {
                        
                    }
                }
            }
        }
    }
    
    private T GetRequiredService<T>()
    {
        return (T)_joaManager.CurrentScope?.ServiceProvider.GetRequiredService(typeof(T))! ?? throw new InvalidOperationException();
    }
}