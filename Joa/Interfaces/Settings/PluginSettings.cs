namespace Interfaces.Settings;

public class PluginSettings
{
    public PluginSettings(string plugin, List<Setting> settings)
    {
        Plugin = plugin;
        Settings = settings;
    }

    public string Plugin { get; set; }
    public List<Setting> Settings { get; set; }
}