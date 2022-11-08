using JoaPluginsPackage;
using JoaPluginsPackage.Attributes;
using JoaPluginsPackage.Injectables;

namespace BookmarksSearch;

[Plugin("Bookmark Search", "", "", "", "")]
public class BookmarksSearch : IIndexablePlugin
{
    private readonly BookmarksSearchSetting _setting;
    private readonly IJoaLogger _joaLogger;
    private readonly IIconHelper _iconHelper;

    public BookmarksSearch(BookmarksSearchSetting setting, IJoaLogger joaLogger, IIconHelper iconHelper)
    {
        _setting = setting;
        _joaLogger = joaLogger;
        _iconHelper = iconHelper;
    }
    
    public void ConfigurePlugin(IPluginBuilder builder)
    {
        builder.AddGlobalProvider<BookmarksProvider>();
    }

    public List<ISearchResult> CachedResutls { get; set; }
    
    public void UpdateIndexes()
    {
        CachedResutls.Clear();
        
        var bookmarks = _setting.Browsers.Where(x => x.Enabled)
            .SelectMany(browser => browser.GetBookmarks(_joaLogger)
                .Select(bookmark => (bookmark, browser)))
            .DistinctBy(x => x.bookmark.url).ToList();

        CachedResutls = bookmarks.Select(x => new BookmarkSerachResult
        {
            Caption = x.bookmark.name,
            Description = x.bookmark.url,
            Icon = _iconHelper.CreateIconFromFileIfNotExists<BookmarksSearch>(x.browser.BrowserLocation)
        }).Cast<ISearchResult>().ToList();
    }
}