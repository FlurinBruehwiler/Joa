using JoaPluginsPackage;
using JoaPluginsPackage.Plugin;
using JoaPluginsPackage.Plugin.Search;
using JoaPluginsPackage.Settings.Attributes;

namespace BookmarksSearch;

[Plugin("Bookmark Search", "", "", "", "")]
public class BookmarksSearch : IGlobalSearchPlugin
{
    private readonly IBrowserHelper _browserHelper;
    
    [SettingProperty]
    public List<Browser> Browsers { get; set; } = new()
    {
        DefaultBrowsers.Chrome,
        DefaultBrowsers.Firefox,
        DefaultBrowsers.Brave,
        DefaultBrowsers.Edge
    };
    
    public List<ISearchResult> GlobalSearchResults { get; set; }

    
    private readonly List<IContextAction> _actions = new()
    {
        new ContextAction("enter", "Open", null, null),
    };

    public BookmarksSearch(IBrowserHelper browserHelper)
    {
        _browserHelper = browserHelper;
    }
    
    public void Execute(ISearchResult searchResult, IContextAction contextAction)
    {
        if(searchResult is not SearchResult result)
            return;

        _browserHelper.OpenWebsite(result.Description);
    }

    public void UpdateIndex()
    {
        var bookmarks = Browsers.Where(x => x.Enabled).SelectMany(x => x.GetBookmarks()).DistinctBy(x => x.url).ToList();

        GlobalSearchResults = bookmarks.Select(x => new SearchResult
        {
            Caption = x.name,
            Description = x.url,
            Icon = "",
            Actions = _actions
        }).Cast<ISearchResult>().ToList();
    }
}