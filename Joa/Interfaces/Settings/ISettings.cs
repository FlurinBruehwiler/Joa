namespace Interfaces.Settings;

public interface ISettings
{
    public IEnumerable<PluginSetting> PluginSettings { get; set; }
}