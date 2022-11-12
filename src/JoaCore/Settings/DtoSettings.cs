using System.Text.Json;
using JoaCore.PluginCore;

namespace JoaCore.Settings;

public class DtoSettings
{
    public Dictionary<string, DtoPluginSettings> Plugins { get; set; }
    
    public DtoSettings(List<PluginDefinition> pluginDefinitions)
    {
        Plugins = pluginDefinitions.ToDictionary(
            pluginDefinition => pluginDefinition.PluginInfo.Name,
            pluginDefinition =>
            {
                return new DtoPluginSettings
                {
                    IsEnabled = true,
                    Setting = JsonSerializer.SerializeToElement(pluginDefinition.Setting, pluginDefinition.Setting.GetType())
                };
            });
    }
}

public class DtoPluginSettings
{
    public bool IsEnabled { get; set; }
    public JsonElement Setting { get; set; }
}