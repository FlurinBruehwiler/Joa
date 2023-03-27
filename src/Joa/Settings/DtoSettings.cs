using System.Text.Json;
using Joa.PluginCore;

namespace Joa.Settings;

public class DtoSettings
{
    public Dictionary<string, DtoPluginSettings> Plugins { get; set; } = null!;
    public GeneralSettings GeneralSettings { get; set; }

    public DtoSettings(List<PluginDefinition> pluginDefinitions, GeneralSettings generalSettings)
    {
        GeneralSettings = generalSettings;
        Plugins = pluginDefinitions.ToDictionary(
            pluginDefinition => pluginDefinition.Manifest.Id,
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