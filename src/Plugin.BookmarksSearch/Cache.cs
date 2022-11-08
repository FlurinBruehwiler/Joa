using JoaPluginsPackage;
using JoaPluginsPackage.Injectables;

namespace BookmarksSearch;

public class Cache : ICache
{
    private readonly Setting _setting;
    private readonly IJoaLogger _joaLogger;
    private readonly IIconHelper _iconHelper;

    public Cache(Setting setting, IJoaLogger joaLogger, IIconHelper iconHelper)
    {
        _setting = setting;
        _joaLogger = joaLogger;
        _iconHelper = iconHelper;
    }
    
    public List<ISearchResult> SearchResults { get; set; }
    
    public void UpdateIndexes()
    {
        SearchResults.Clear();
        
        var bookmarks = _setting.Browsers.Where(x => x.Enabled)
            .SelectMany(browser => browser.GetBookmarks(_joaLogger)
                .Select(bookmark => (bookmark, browser)))
            .DistinctBy(x => x.bookmark.url).ToList();

        SearchResults = bookmarks.Select(x => new SerachResult
        {
            Caption = x.bookmark.name,
            Description = x.bookmark.url,
            Icon = _iconHelper.CreateIconFromFileIfNotExists<BookmarksSearch>(x.browser.BrowserLocation)
        }).Cast<ISearchResult>().ToList();
    }
}