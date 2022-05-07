using Interfaces;
using Interfaces.Settings;
using Newtonsoft.Json;

namespace JoaCore;

public class CoreSettings : IJoaSettings
{
    private readonly string _settingsFilePath;

    public CoreSettings()
    {
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

    public void UpdateSettings()
    {
        //ToDo        
        SaveSettings();
    }

    private void SaveSettings()
    {
        //ToDo
    }
}