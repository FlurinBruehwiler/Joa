using Interfaces.Settings;
using Interfaces.Settings.Attributes;

namespace ApplicationSearch;

public class FileExtension
{
    [SettingProperty]
    public string Extensions { get; set; } = null!;
}