using JoaPluginsPackage;
using JoaPluginsPackage.Providers;

namespace BookmarksSearch;

public class BookmarksProvider : IProvider
{
    private readonly BookmarksSearch _bookmarksSearch;

    public BookmarksProvider(BookmarksSearch bookmarksSearch)
    {
        _bookmarksSearch = bookmarksSearch;
    }

    public List<ISearchResult> GetSearchResults(string searchString)
    {
        return _bookmarksSearch.CachedResutls;
    }
}