using Joa.Api.Attributes;
using Joa.Api;
using OperatingSystem = Joa.Api.Enums.OperatingSystem;

namespace ApplicationSearch;

public class Settings : ISetting
{
    [SettingProperty(Name = "Web Search Engines")]
    public List<Folder> Folders { get; set; } = new()
    {
        new Folder
        {
            Path = Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.CommonApplicationData), 
                @"Microsoft\Windows\Start Menu\Programs"), 
            OperatingSystem = OperatingSystem.Windows
        },
        new Folder
        {
            Path = Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.ApplicationData), 
                @"Microsoft\Windows\Start Menu"), 
            OperatingSystem = OperatingSystem.Windows
        },
        new Folder
        {
            Path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop), 
            OperatingSystem = OperatingSystem.Windows
        }
    };

    [SettingProperty]
    public List<FileExtension> Extensions { get; set; } = new()
    {
        new FileExtension {Extension = ".lnk"},
        new FileExtension {Extension = ".appref-ms"},
        new FileExtension {Extension = ".url"},
        new FileExtension {Extension = ".exe"},
    };

    [SettingProperty] 
    public bool ShowFullFilePath { get; set; } = false;

    [SettingProperty] 
    public bool UseNativeIcons { get; set; } = true;
}