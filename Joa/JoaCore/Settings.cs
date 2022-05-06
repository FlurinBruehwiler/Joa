using Interfaces;
using Interfaces.Settings;
using Newtonsoft.Json;

namespace JoaCore;

public class Settings : ISettings
{
    public IEnumerable<PluginSetting> PluginSettings { get; set; }

    private readonly string _settingsFilePath;

    public Settings(IEnumerable<PluginSetting> pluginSettings)
    {
        PluginSettings = pluginSettings;
        _settingsFilePath =
            Path.GetFullPath(Path.Combine(typeof(PluginLoader).Assembly.Location, @"..\..\..\..\..\settings.json"));
        ReadSettings();
    }

    private void ReadSettings()
    {
        var json = File.ReadAllText(_settingsFilePath);
        if (json == string.Empty)
            return;

        //ToDo
    }

    public void UpdateSettings(PluginLoader pluginLoader)
    {
        PluginSettings = pluginLoader.GetPluginSettings();
        
        SaveSettings();
    }

    private void SaveSettings()
    {
        var json = JsonConvert.SerializeObject(PluginSettings);
        File.WriteAllText(_settingsFilePath, json);
    }
}