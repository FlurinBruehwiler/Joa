using JoaLauncher.Api;
using JoaLauncher.Api.Injectables;
using JoaLauncher.Api.Plugin;
using JoaLauncher.Api.Providers;
using Microsoft.Extensions.Logging;

namespace BookmarksSearch;

public class BookmarksSearch : ICache, IProvider, IPlugin
{
    private readonly Setting _setting;
    private readonly ILogger<BookmarksSearch> _logger;
    private readonly IIconHelper _iconHelper;
    private List<SearchResult> _searchResults = new();

    public BookmarksSearch(Setting setting, ILogger<BookmarksSearch> logger, IIconHelper iconHelper)
    {
        _setting = setting;
        _logger = logger;
        _iconHelper = iconHelper;
    }

    public void UpdateIndexes()
    {
        _searchResults.Clear();

        foreach (var settingBrowser in _setting.Browsers)
        {
            settingBrowser.Icon =
                _iconHelper.CreateIconFromFileIfNotExists<BookmarksSearch>(settingBrowser.BrowserLocation);
        }
        
        var bookmarks = _setting.Browsers.Where(x => x.Enabled)
            .SelectMany(browser => browser.GetBookmarks(_logger)
                .Select(bookmark => (bookmark, browser)))
            .DistinctBy(x => x.bookmark.url).ToList();

        _searchResults = bookmarks.Select(x => new SerachResult
        {
            Title = x.bookmark.name,
            Description = x.bookmark.url,
            Icon = x.browser.Icon!
        }).Cast<SearchResult>().ToList();
    }

    public List<SearchResult> GetSearchResults(string searchString)
    {
        return _searchResults;
    }

    public void ConfigurePlugin(IPluginBuilder builder)
    {
        builder.AddGlobalProvider<BookmarksSearch>();
    }
}