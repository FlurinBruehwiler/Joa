using Interfaces.Settings;

namespace ApplicationSearch;

public class ApplicationFolder
{
    [SettingProperty(SettingType = SettingType.Path)]
    public string Path { get; set; } = null!;
}