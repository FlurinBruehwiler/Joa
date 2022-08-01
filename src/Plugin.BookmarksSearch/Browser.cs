using System.Text.Json;
using JoaPluginsPackage.Settings.Attributes;
using OperatingSystem = JoaPluginsPackage.OperatingSystem;

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
    public string MacOSLocation { get; set; } = null!;

    [SettingProperty]
    [OperatingSystem(OperatingSystem.Linux)]
    public string LinuxLocation { get; set; } = null!;

    public List<Bookmark> GetBookmarks()
    {
        if (!File.Exists(WindowsLocation))
            return new List<Bookmark>();

        var content = File.ReadAllText(WindowsLocation);
        
        var bookmarksFile = JsonSerializer.Deserialize<BookmarksFileModel>(content);

        if(bookmarksFile is null)
            return new List<Bookmark>();

        return bookmarksFile.roots.bookmark_bar.children;
    }
}