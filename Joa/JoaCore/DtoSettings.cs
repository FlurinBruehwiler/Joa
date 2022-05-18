using System.Text.Json;
using Microsoft.VisualBasic.CompilerServices;

namespace JoaCore;

public class DtoSettings
{
    private Dictionary<string, Dictionary<string, object>> _pluginSettings;

    public Dictionary<string, Dictionary<string, object>> PluginSettings
    {
        get => _pluginSettings;
        set
        {
            Dictionary<string, Dictionary<string, object>> newPluginSettings = new();

            foreach (var (key, pluginSetting) in value)
            {
                newPluginSettings.Add(key, ConvertPlugin(pluginSetting));
            }

            _pluginSettings = newPluginSettings;
        }
    }
    
    public DtoSettings(Settings setting)
    {
        _pluginSettings = setting.PluginDefinitions.ToDictionary(pluginDefinition => pluginDefinition.Name,
            pluginDefinition => pluginDefinition.SettingsCollection.PluginSettings.ToDictionary(pluginSetting => pluginSetting.Name,
                pluginSetting => pluginSetting.Value));
    }

    public DtoSettings()
    {
        
    }

    Dictionary<string, object> ConvertPlugin(Dictionary<string, object> pluginSettings)
    {
        Dictionary<string, object> newDictionary = new();
            
        foreach (var (key, settingValue) in pluginSettings)
        {
            if (settingValue is not JsonElement jsonElement)
            {
                newDictionary.Add(key, settingValue);
                continue;
            }
                
            var type = jsonElement.ValueKind switch
            {
                JsonValueKind.Number => typeof(int),
                JsonValueKind.String => typeof(string),
                JsonValueKind.True => typeof(bool),
                JsonValueKind.False => typeof(bool),
                JsonValueKind.Array => typeof(List<Dictionary<string, object>>),
                _ => throw new JsonException()
            };

            var newValue = jsonElement.Deserialize(type) ?? throw new JsonException();

            if (newValue is List<Dictionary<string, object>> list)
            {
                newValue = list.Select(ConvertListItem).ToList();
            }
                
            newDictionary.Add(key, newValue);
        }

        return newDictionary;
    }

    Dictionary<string, object> ConvertListItem(Dictionary<string, object> settings)
    {
        Dictionary<string, object> newDictionary = new();
            
        foreach (var (key, settingValue) in settings)
        {
            if (settingValue is not JsonElement jsonElement)
            {
                newDictionary.Add(key, settingValue);
                continue;
            }
                
            var type = jsonElement.ValueKind switch
            {
                JsonValueKind.Number => typeof(int),
                JsonValueKind.String => typeof(string),
                JsonValueKind.True => typeof(bool),
                JsonValueKind.False => typeof(bool),
                _ => throw new JsonException()
            };

            newDictionary.Add(key, jsonElement.Deserialize(type) ?? throw new JsonException());
        }

        return newDictionary;
    }
}