using Interfaces.Settings;

namespace JoaCore.Settings;

public class CoreSettings : IJoaSettings
{
    public CoreSettings()
    {
        CoreSettingsItems = new Dictionary<string, CoreSettingsItem>()
        {

        };
    }

    private Dictionary<string, CoreSettingsItem> CoreSettingsItems { get; set; }
    
    public object? GetSetting(string key)
    {
        if (CoreSettingsItems.TryGetValue(key, out var value))
            return value.Setting;

        return null;
    }
}