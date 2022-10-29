using JoaPluginsPackage;

namespace JoaCore;

public static class PluginExtensions
{
    public static List<PluginSearchResult> ToPluginSerachResults(this List<ISearchResult> searchResults)
    {
        return searchResults.Select(x => new PluginSearchResult
        {
            SearchResult = x
        }).ToList();
    }
}