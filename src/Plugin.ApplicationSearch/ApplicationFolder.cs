using JoaPluginsPackage.Settings.Attributes;

namespace ApplicationSearch;

public class ApplicationFolder
{
    [Path]
    [SettingProperty]
    public string Path { get; set; } = null!;
}