using JoaPluginsPackage.Settings.Attributes;

namespace ApplicationSearch;

public class FileExtension
{
    [SettingProperty]
    public string Extension { get; set; } = null!;
}