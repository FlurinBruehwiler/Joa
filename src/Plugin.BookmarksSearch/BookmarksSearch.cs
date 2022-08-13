using JoaPluginsPackage;
using JoaPluginsPackage.Attributes;
using JoaPluginsPackage.Injectables;
using JoaPluginsPackage.Plugin;

namespace BookmarksSearch;

[Plugin("Bookmark Search", "", "", "", "")]
public class BookmarksSearch : IGlobalSearchPlugin
{
    private readonly IBrowserHelper _browserHelper;
    private readonly IJoaLogger _joaLogger;

    [SettingProperty]
    public List<Browser> Browsers { get; set; } = new()
    {
        DefaultBrowsers.Chrome,
        DefaultBrowsers.Firefox,
        DefaultBrowsers.Brave,
        DefaultBrowsers.Edge
    };
    
    public List<SearchResult> GlobalSearchResults { get; set; } = null!;

    public BookmarksSearch(IBrowserHelper browserHelper, IJoaLogger joaLogger)
    {
        _browserHelper = browserHelper;
        _joaLogger = joaLogger;
    }
    
    public void Execute(SearchResult searchResult, ContextAction contextAction)
    {
        _browserHelper.OpenWebsite(searchResult.Description);
    }

    public void UpdateIndex()
    {
        var bookmarks = Browsers.Where(x => x.Enabled).SelectMany(x => x.GetBookmarks(_joaLogger)).DistinctBy(x => x.url).ToList();

        GlobalSearchResults = bookmarks.Select(x => new SearchResult
        {
            Caption = x.name,
            Description = x.url,
            Icon = "",
        }).ToList();
    }
}