using System.Text.Json;
using JoaInterface.PluginCore;

namespace JoaInterface.Settings;

public class DtoSettings
{
    public Dictionary<string, DtoPluginSettings> Plugins { get; set; } = null!;
    
    public DtoSettings(List<PluginDefinition> pluginDefinitions)
    {
        Plugins = pluginDefinitions.ToDictionary(
            pluginDefinition => pluginDefinition.PluginInfo.Name,
            pluginDefinition => new DtoPluginSettings
            {
                IsEnabled = true,
                Setting = JsonSerializer.SerializeToElement(pluginDefinition.Setting, pluginDefinition.Setting.GetType())
            });
    }

    public DtoSettings() { }
}

public class DtoPluginSettings
{
    public bool IsEnabled { get; set; }
    public JsonElement Setting { get; set; }
}