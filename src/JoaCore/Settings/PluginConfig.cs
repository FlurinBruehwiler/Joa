using JoaPluginsPackage.Attributes;

namespace JoaCore.Settings;

public record PluginConfig
{
    public PluginAttribute PluginAttribute { get; init; }
    public IEnumerable<PluginSetting> PluginSettings { get; init; }
}