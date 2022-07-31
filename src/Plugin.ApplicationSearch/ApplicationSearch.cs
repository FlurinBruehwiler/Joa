using System.Diagnostics;
using System.Text.Json;
using JoaPluginsPackage.Logger;
using JoaPluginsPackage.Plugin;
using JoaPluginsPackage.Plugin.Search;
using JoaPluginsPackage.Settings.Attributes;

namespace ApplicationSearch;

public class ApplicationSearch : IGlobalSearchPlugin
{
    private readonly IJoaLogger _joaLogger;
    public List<ISearchResult> GlobalSearchResults { get; set; }
    
    private readonly List<IContextAction> _actions = new()
    {
        new ContextAction("enter", "Open", null, null),
    };


    public ApplicationSearch(IJoaLogger joaLogger)
    {
        _joaLogger = joaLogger;
        GlobalSearchResults = new List<ISearchResult>();
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
                GlobalSearchResults.Add(new SearchResult(Path.GetFileNameWithoutExtension(path), "", "", _actions, path));
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
        new FileExtension {Extension = ".Ink"},
        new FileExtension {Extension = ".appref-ms"},
        new FileExtension {Extension = ".url"},
        new FileExtension {Extension = ".exe"},
    };

    [SettingProperty] public bool ShowFullFilePath { get; set; } = false;

    [SettingProperty] public bool UseNativeIcons { get; set; } = true;

    public void Execute(ISearchResult sr, IContextAction contextAction)
    {
        if (sr is not SearchResult searchResult)
            return;
        
        _joaLogger.Log(searchResult.FilePath, IJoaLogger.LogLevel.Info);
        
        var info = new ProcessStartInfo ( searchResult.FilePath )
        {
            UseShellExecute = true
        };
        Process.Start(info);
    }

    public string Name => "";
    public string Description => "";
    public string Version => "";
    public string Author => "";
    public string SourceCode => "";
    public Guid Id { get; } = Guid.NewGuid();
}