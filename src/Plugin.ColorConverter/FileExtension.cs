using JoaPluginsPackage.Attributes;

namespace ColorConverter;

public class FileExtension
{
    [SettingProperty]
    public string Extensions { get; set; } = null!;
}