using Interfaces;
using Interfaces.Plugin;
using Interfaces.Settings;
using Interfaces.Settings.Attributes;

namespace ApplicationSearch;

public class ApplicationSearch : IPlugin, IIndexable
{
    public ApplicationSearch()
    {
        _cachedResults = new List<ISearchResult>();
    }
    
    public void UpdateIndex()
    {
        _cachedResults.Clear();

        var files = new List<string>();

        foreach (var applicationFolder in Folders)
        {
            files.AddRange(Directory.GetFiles(applicationFolder.Path, "*", SearchOption.AllDirectories));
        }

        foreach (var file in files)
        {
            if (Extensions.Any(x => file.EndsWith(x.Extension)))
            {
                _cachedResults.Add(new SearchResult {FilePath = file, Caption = Path.GetFileName(file)});
            }
        }
    }


    [SettingProperty]
    private List<ApplicationFolder> Folders { get; set; } = new()
    {
        new ApplicationFolder {Path = @"C:\ProgramData\Microsoft\Windows\Start Menu\Programs"},
        new ApplicationFolder {Path = @"C:\Users\FBR\AppData\Roaming\Microsoft\Windows\Start Menu"},
        new ApplicationFolder {Path = @"C:\Users\FBR\Desktop"},
    };

    [SettingProperty]
    private List<FileExtension> Extensions { get; set; } = new()
    {
        new FileExtension {Extension = ".Ink"},
        new FileExtension {Extension = ".appref-ms"},
        new FileExtension {Extension = ".url"},
        new FileExtension {Extension = ".exe"},
    };

    [SettingProperty] public bool ShowFullFilePath { get; set; } = false;

    [SettingProperty] public bool UseNativeIcons { get; set; } = true;
    public bool AcceptNonMatchingSearchString => true;
    public bool Validator(string searchString) => true;

    private readonly List<ISearchResult> _cachedResults;

    public List<ISearchResult> GetResults(string searchString)
    {
        return _cachedResults;
    }

    public void Execute(ISearchResult searchResult)
    {

    }

}