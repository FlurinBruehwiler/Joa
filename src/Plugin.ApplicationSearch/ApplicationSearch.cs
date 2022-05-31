using Interfaces.Plugin;
using Interfaces.Settings.Attributes;

namespace ApplicationSearch;

public class ApplicationSearch : IIndexablePlugin
{
    public List<ICommand> SearchResults { get; set; }

    
    public ApplicationSearch()
    {
        SearchResults = new List<ICommand>();
    }
    
    public void UpdateIndex()
    {
        SearchResults.Clear();

        var files = new List<string>();

        foreach (var applicationFolder in Folders)
        {
            files.AddRange(Directory.GetFiles(applicationFolder.Path, "*", SearchOption.AllDirectories));
        }

        foreach (var file in files)
        {
            if (Extensions.Any(x => file.EndsWith(x.Extension)))
            {
                SearchResults.Add(new Command {FilePath = file, Caption = Path.GetFileName(file)});
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

    public void Execute(ICommand command)
    {

    }
}