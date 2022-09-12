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
    public List<ISearchResult> GlobalSearchResults { get; set; } = new();
    
    public ApplicationSearch(IJoaLogger joaLogger)
    {
        _joaLogger = joaLogger;
    }
    
    public void UpdateIndex()
    {
        GlobalSearchResults.Clear();

        var paths = new List<string>();

        foreach (var applicationFolder in Folders)
        {
            if (Directory.Exists(applicationFolder.Path))
            {
                paths.AddRange(Directory.GetFiles(applicationFolder.Path, "*", SearchOption.AllDirectories));
            }
        }

        foreach (var path in paths)
        {
            if (Extensions.Any(x => path.EndsWith(x.Extension, StringComparison.OrdinalIgnoreCase)))
            {
                GlobalSearchResults.Add(new ApplicationSearchResult
                {
                    Caption = Path.GetFileNameWithoutExtension(path),
                    Description = "",
                    Icon = "",
                    FilePath = path
                });
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
}