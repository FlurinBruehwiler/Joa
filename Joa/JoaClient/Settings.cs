using Interfaces.Settings;
using Newtonsoft.Json;

namespace AppWithPlugin;

public class Settings : ISettings
{
    public List<PluginSettings> PluginSettings { get; set; }

    private readonly string _settingsFilePath;
    
    public Settings()
    {
        PluginSettings = new List<PluginSettings>();
        _settingsFilePath = Path.GetFullPath(Path.Combine(typeof(PluginLoader).Assembly.Location, @"..\..\..\..\..\settings.json"));
        ReadSettings();
    }

    public void ReadSettings()
    {
        var json = File.ReadAllText(_settingsFilePath);
        if (json == string.Empty)
            return;
        
        PluginSettings = JsonConvert.DeserializeObject<List<PluginSettings>>(json) ?? throw new Exception();
    }

    public void UpdateSettings(PluginLoader pluginLoader)
    {
        var plugins = pluginLoader.GetPlugins();
        PluginSettings = plugins.Select(x => x.PluginSettings).ToList();
        SaveSettings();
    }

    public void SaveSettings()
    {
        var json = JsonConvert.SerializeObject(PluginSettings);
        File.WriteAllText(_settingsFilePath, json);
    }
}