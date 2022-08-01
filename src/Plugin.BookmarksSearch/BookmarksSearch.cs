using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.Json;
using JoaPluginsPackage.Plugin;
using JoaPluginsPackage.Plugin.Search;

namespace BookmarksSearch;

public class BookmarksSearch : IGlobalSearchPlugin
{
    public string Name { get; }
    public string Description { get; }
    public string Version { get; }
    public string Author { get; }
    public string SourceCode { get; }
    public Guid Id { get; }
    
    private readonly string chromeBookmarksFilePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Google\Chrome\User Data\Default\Bookmarks";
    
    private readonly List<IContextAction> _actions = new()
    {
        new ContextAction("enter", "Open", null, null),
    };
    
    public void Execute(ISearchResult searchResult, IContextAction contextAction)
    {
        if(searchResult is not SearchResult result)
            return;

        OpenBrowser(result.Description);
    }

    public List<ISearchResult> GlobalSearchResults { get; set; }
    public void UpdateIndex()
    {
        if (!File.Exists(chromeBookmarksFilePath))
            return;

        var content = File.ReadAllText(chromeBookmarksFilePath);
        
        var bookmarksFile = JsonSerializer.Deserialize<BookmarksFileModel>(content);

        if(bookmarksFile is null)
            return;

        var bookmarks = bookmarksFile.roots.bookmark_bar.children;

        GlobalSearchResults = bookmarks.Select(x => new SearchResult
        {
            Caption = x.name,
            Description = x.url,
            Icon = "",
            Actions = _actions
        }).Cast<ISearchResult>().ToList();
    }
    
    public static void OpenBrowser(string url)
    {
        try
        {
            Process.Start(url);
        }
        catch
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                url = url.Replace("&", "^&");
                Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("xdg-open", url);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", url);
            }
            else
            {
                throw;
            }
        }
    }
}