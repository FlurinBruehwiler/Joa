using System.Text.Json;
using JoaPluginsPackage.Attributes;
using JoaPluginsPackage.Injectables;
using OperatingSystem = JoaPluginsPackage.Enums.OperatingSystem;

namespace BookmarksSearch;

public class Browser
{
    [SettingProperty]
    public string Name { get; set; } = null!;

    [Enabler]
    [SettingProperty]
    public bool Enabled { get; set; }
    
    [SettingProperty]
    [OperatingSystem(OperatingSystem.Windows)]
    public string WindowsLocation { get; set; } = null!;

    [SettingProperty]
    [OperatingSystem(OperatingSystem.MacOS)]
    public string MacOsLocation { get; set; } = null!;

    [SettingProperty]
    [OperatingSystem(OperatingSystem.Linux)]
    public string LinuxLocation { get; set; } = null!;

    [SettingProperty]
    public string BrowserLocation { get; set; } = null!;
    
    public List<Bookmark> GetBookmarks(IJoaLogger joaLogger)
    {
        var bookmarkLocation =
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + WindowsLocation;
        
        if (!File.Exists(bookmarkLocation))
            return new List<Bookmark>();

        var content = File.ReadAllText(bookmarkLocation);
        
        var bookmarksFile = JsonSerializer.Deserialize<BookmarksFileModel>(content);

        if(bookmarksFile is null)
            return new List<Bookmark>();

        return bookmarksFile.roots.bookmark_bar.children;
    }
}