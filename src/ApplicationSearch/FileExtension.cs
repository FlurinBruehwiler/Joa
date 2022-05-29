using Interfaces.Settings;
using Interfaces.Settings.Attributes;

namespace ApplicationSearch;

public class FileExtension
{
    [SettingProperty]
    public string Extension { get; set; } = null!;
}