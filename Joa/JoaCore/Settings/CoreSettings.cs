using Interfaces.Settings;
using JoaCore.PluginCore;

namespace JoaCore.Settings;

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
        //var json = File.ReadAllText(_settingsFilePath);

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