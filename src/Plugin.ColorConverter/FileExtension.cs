using JoaPluginsPackage.Settings.Attributes;

namespace ApplicationSearch;

public class FileExtension
{
    [SettingProperty]
    public string Extensions { get; set; } = null!;
}