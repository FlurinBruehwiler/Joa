namespace Interfaces.Settings;

public class PluginSetting
{
    public PluginSetting(List<Setting> settings)
    {
        Settings = settings;
    }

    public List<Setting> Settings { get; set; }
}