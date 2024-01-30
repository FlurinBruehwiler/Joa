namespace BookmarksSearch;

public static class DefaultBrowsers
{
    public static Browser Chrome = new()
    {
        Name = "Chrome",
        Enabled = true,
        Location = @"\Google\Chrome\User Data\Default\Bookmarks",
        BrowserLocation = @"C:\ProgramData\Microsoft\Windows\Start Menu\Programs\Google Chrome.lnk"
    };

    public static Browser Firefox = new()
    {
        Name = "Firefox",
        Enabled = false,
        Location = @"",
        BrowserLocation = ""
    };

    public static Browser Brave = new()
    {
        Name = "Brave",
        Enabled = false,
        Location = @"",
        BrowserLocation = ""
    };


    public static Browser Edge = new()
    {
        Name = "Edge",
        Enabled = false,
        Location = @"",
        BrowserLocation = ""
    };
}