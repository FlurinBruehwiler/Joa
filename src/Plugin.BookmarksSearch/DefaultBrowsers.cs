namespace BookmarksSearch;

public static class DefaultBrowsers
{
    public static Browser Chrome = new()
    {
        Name = "Chrome",
        Enabled = true,
        LinuxLocation = "",
        WindowsLocation = @"\Google\Chrome\User Data\Default\Bookmarks",
        MacOsLocation = "/Google/Chrome/Default/Bookmarks"
    };
    
    public static Browser Firefox = new()
    {
        Name = "Firefox",
        Enabled = false,
        LinuxLocation = "",
        WindowsLocation = @"",
        MacOsLocation = ""
    };
    
    public static Browser Brave = new()
    {
        Name = "Brave",
        Enabled = false,
        LinuxLocation = "",
        WindowsLocation = @"",
        MacOsLocation = ""
    };
    
    
    public static Browser Edge = new()
    {
        Name = "Edge",
        Enabled = false,
        LinuxLocation = "",
        WindowsLocation = @"",
        MacOsLocation = ""
    };
}