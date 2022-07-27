using JoaPluginsPackage.Plugin;
using JoaPluginsPackage.Plugin.Search;
using JoaPluginsPackage.Settings.Attributes;

namespace ApplicationSearch;

public class ApplicationSearch : IGlobalSearchPlugin
{
    public List<ISearchResult> GlobalSearchResults { get; set; }

    
    public ApplicationSearch()
    {
        GlobalSearchResults = new List<ISearchResult>();
    }
    
    public void UpdateIndex()
    {
        GlobalSearchResults.Clear();

        var files = new List<string>();

        foreach (var applicationFolder in Folders)
        {
            files.AddRange(Directory.GetFiles(applicationFolder.Path, "*", SearchOption.AllDirectories));
        }

        foreach (var file in files)
        {
            if (Extensions.Any(x => file.EndsWith(x.Extension)))
            {
                GlobalSearchResults.Add(new SearchResult("", "", "", new List<IAction>(), file));
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

    public void Execute(ISearchResult searchResult, IAction action)
    {

    }

    public string Name => "";
    public string Description => "";
    public string Version => "";
    public string Author => "";
    public string SourceCode => "";
    public Guid Id => Guid.NewGuid();
}