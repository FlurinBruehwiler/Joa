using Interfaces.Settings;

namespace ApplicationSearch;

public class FileExtension
{
    [SettingProperty]
    public string Extensions { get; set; }
}