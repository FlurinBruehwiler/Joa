using JoaPluginsPackage;
using JoaPluginsPackage.Enums;
using JoaPluginsPackage.Injectables;
using JoaPluginsPackage.Providers;

namespace BookmarksSearch;

public class BookmarksProvider : IProvider
{
    private readonly BookmarksSearchSetting _setting;
    private readonly IJoaLogger _joaLogger;
    private readonly IIconHelper _iconHelper;

    public BookmarksProvider(BookmarksSearchSetting setting, IJoaLogger joaLogger, IIconHelper iconHelper)
    {
        _setting = setting;
        _joaLogger = joaLogger;
        _iconHelper = iconHelper;
    }

    public List<ISearchResult> SearchResults { get; set; }
    public SearchResultLifetime SearchResultLifetime { get; set; }
    public void UpdateSearchResults(string searchString)
    {
        var bookmarks = _setting.Browsers.Where(x => x.Enabled)
            .SelectMany(browser => browser.GetBookmarks(_joaLogger)
                .Select(bookmark => (bookmark, browser)))
            .DistinctBy(x => x.bookmark.url).ToList();

        var x = bookmarks.Select(x => new BookmarkSerachResult
        {
            Caption = x.bookmark.name,
            Description = x.bookmark.url,
            Icon = _iconHelper.CreateIconFromFileIfNotExists<BookmarksSearch>(x.browser.BrowserLocation)
        }).ToList();

        SearchResults = x.Cast<ISearchResult>().ToList();
    }
}