using Interfaces;
using Interfaces.Plugin;
using Interfaces.Settings;
using Interfaces.Settings.Attributes;

namespace ApplicationSearch;

public class ApplicatioinSearch : IPlugin
{
    [SettingProperty]
    public List<ApplicationFolder> Folders { get; set; } = new()
    {
        new ApplicationFolder { Path = @"C:\ProgramData\Microsoft\Windows\Start Menu\Programs" },
        new ApplicationFolder { Path = @"C:\Users\FBR\AppData\Roaming\Microsoft\Windows\Start Menu" },
        new ApplicationFolder { Path = @"C:\Users\FBR\Desktop" },
    };

    [SettingProperty]
    public List<FileExtension> Extensions { get; set; } = new()
    {
        new FileExtension { Extensions = ".Ink" },
        new FileExtension { Extensions = ".appref-ms" },
        new FileExtension { Extensions = ".url" },
        new FileExtension { Extensions = ".exe" },
    };
    
    [SettingProperty]
    public bool ShowFullFilePath { get; set; } = false;
    
    [SettingProperty]
    public bool UseNativeIcons { get; set; } = true;
    public bool AcceptNonMatchingSearchString => true;
    public List<Func<string, bool>> Matchers => new();
    public List<ISearchResult> GetResults(string searchString)
    {
        return new List<ISearchResult>();
    }

    public void Execute(ISearchResult searchResult)
    {
        
    }
}