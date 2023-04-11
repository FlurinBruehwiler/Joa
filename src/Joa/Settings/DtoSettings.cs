using System.Text.Json;
using Joa.BuiltInPlugin;
using Joa.PluginCore;

namespace Joa.Settings;

public class DtoSettings
{
    public Dictionary<string, DtoPluginSettings> Plugins { get; set; } = null!;
    public BuiltInSettings BuiltInSettings { get; set; }

    public DtoSettings(List<PluginDefinition> pluginDefinitions, BuiltInSettings builtInSettings)
    {
        BuiltInSettings = builtInSettings;
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