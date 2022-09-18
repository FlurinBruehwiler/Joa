using JoaPluginsPackage;
using JoaPluginsPackage.Attributes;
using JoaPluginsPackage.Injectables;
using JoaPluginsPackage.Plugin;

namespace BookmarksSearch;

[Plugin("Bookmark Search", "", "", "", "")]
public class BookmarksSearch : IGlobalSearchPlugin
{
    private readonly IJoaLogger _joaLogger;
    private readonly IIconHelper _iconHelper;

    [SettingProperty]
    public List<Browser> Browsers { get; set; } = new()
    {
        DefaultBrowsers.Chrome,
        DefaultBrowsers.Firefox,
        DefaultBrowsers.Brave,
        DefaultBrowsers.Edge
    };
    
    public List<ISearchResult> GlobalSearchResults { get; set; } = null!;

    public BookmarksSearch(IJoaLogger joaLogger, IIconHelper iconHelper)
    {
        _joaLogger = joaLogger;
        _iconHelper = iconHelper;
    }

    public void UpdateIndex()
    {
        var bookmarks = Browsers.Where(x => x.Enabled)
            .SelectMany(browser => browser.GetBookmarks(_joaLogger)
                .Select(bookmark => (bookmark, browser)))
            .DistinctBy(x => x.bookmark.url).ToList();

        var x = bookmarks.Select(x => new BookmarkSerachResult
        {
            Caption = x.bookmark.name,
            Description = x.bookmark.url,
            Icon = GetIconForBrowser(x.browser)
        }).ToList();

        GlobalSearchResults = x.Cast<ISearchResult>().ToList();
    }

    public string GetIconForBrowser(Browser browser)
    {
        var iconLocation = Path.Combine(_iconHelper.GetIconsDirectory(typeof(BookmarksSearch)), browser.Name);

        _iconHelper.CreateIconFromFileIfNotExists(iconLocation, browser.BrowserLocation);

        return iconLocation;
    }
}