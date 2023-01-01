using System.Collections;
using System.Diagnostics;
using System.Net;
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
        var res = new DtoFrontendSettings();
        
        foreach (var x in GetRequiredService<PluginManager>().Plugins)
        {
            foreach (var y in x.SettingConfiguration.PropertyDefinitions)
            {
                if (y is SettingsProperty settingsProperty)
                {
                    var dtoSettingsDescription = new DtoPropertyDescription
                    {
                        Name = settingsProperty.PropertyInfo.Name,
                        Attributes = settingsProperty.PropertyInfo.CustomAttributes.ToList()
                    };
                    
                    res.SettingsDescriptions.Add(dtoSettingsDescription);
                    
                    res.Properties.Add(new DtoPropertyInstance
                    {
                        Value = settingsProperty.GetValue(),
                        SettingsDescriptionId = dtoSettingsDescription.Id
                    }); 
                    
                }
                
                if (y is SettingsPropertyList settingsPropertyList)
                {
                    var dtoSettingsDescription = new DtoPropertyDescription
                    {
                        Name = settingsPropertyList.PropertyInfo.Name,
                        Attributes = settingsPropertyList.PropertyInfo.CustomAttributes.ToList()
                    };
                    
                    res.SettingsDescriptions.Add(dtoSettingsDescription);
                    
                    res.Properties.Add(new DtoPropertyInstance
                    {
                        SettingsDescriptionId = dtoSettingsDescription.Id,
                        Value = settingsPropertyList
                    }); 
                    
                    foreach (var propertyInfo in settingsPropertyList.SettingsProperties)
                    {
                        var a = new DtoPropertyDescription
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