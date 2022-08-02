using System.Diagnostics;
using System.Text.Json;
using JoaPluginsPackage;
using JoaPluginsPackage.Attributes;
using JoaPluginsPackage.Injectables;
using JoaPluginsPackage.Plugin;

namespace ApplicationSearch;

[Plugin("Application Search", "", "", "", "")]
public class ApplicationSearch : IGlobalSearchPlugin
{
    private readonly IJoaLogger _joaLogger;
    public List<SearchResult> GlobalSearchResults { get; set; }
    
    public ApplicationSearch(IJoaLogger joaLogger)
    {
        _joaLogger = joaLogger;
        GlobalSearchResults = new List<SearchResult>();
    }
    
    public void UpdateIndex()
    {
        GlobalSearchResults.Clear();

        var paths = new List<string>();

        foreach (var applicationFolder in Folders)
        {
            paths.AddRange(Directory.GetFiles(applicationFolder.Path, "*", SearchOption.AllDirectories));
        }

        foreach (var path in paths)
        {
            if (Extensions.Any(x => path.EndsWith(x.Extension, StringComparison.OrdinalIgnoreCase)))
            {
                GlobalSearchResults.Add(new ApplicationSearchResult(Path.GetFileNameWithoutExtension(path), "", "", path));
            }
        }
        
        _joaLogger.Log(JsonSerializer.Serialize(GlobalSearchResults), IJoaLogger.LogLevel.Info);
    }

    [SettingProperty(Name = "Web Search Engines")]
    public List<ApplicationFolder> Folders { get; set; } = new()
    {
        new ApplicationFolder {Path = @"C:\ProgramData\Microsoft\Windows\Start Menu\Programs"},
        new ApplicationFolder {Path = @"C:\Users\FBR\AppData\Roaming\Microsoft\Windows\Start Menu"},
        new ApplicationFolder {Path = @"C:\Users\FBR\Desktop"},
    };

    [SettingProperty]
    public List<FileExtension> Extensions { get; set; } = new()
    {
        new FileExtension {Extension = ".lnk"},
        new FileExtension {Extension = ".appref-ms"},
        new FileExtension {Extension = ".url"},
        new FileExtension {Extension = ".exe"},
    };

    [SettingProperty] public bool ShowFullFilePath { get; set; } = false;

    [SettingProperty] public bool UseNativeIcons { get; set; } = true;

    public void Execute(SearchResult sr, ContextAction contextAction)
    {
        if (sr is not ApplicationSearchResult searchResult)
            return;
        
        _joaLogger.Log(searchResult.FilePath, IJoaLogger.LogLevel.Info);
        
        var info = new ProcessStartInfo ( searchResult.FilePath )
        {
            UseShellExecute = true
        };
        Process.Start(info);
    }
}