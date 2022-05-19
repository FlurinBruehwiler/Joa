using Interfaces;
using Interfaces.Settings;

namespace ApplicationSearch;

public class ApplicatioinSearch : IPlugin
{
    public ApplicatioinSearch()
    {
        ID = new Guid();
    }

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
    
    public Guid ID { get; }
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