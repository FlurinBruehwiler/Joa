using JoaLauncher.Api;
using JoaInterface.SearchEngine;

namespace JoaInterface.PluginCore;

public static class PluginExtensions
{
    static PluginExtensions()
    {
        StringMatcher.Instance = new StringMatcher();
    }
    
    public static List<PluginSearchResult> ToPluginSerachResults(this List<ISearchResult> searchResults)
    {
        return searchResults.Select(x => new PluginSearchResult
        {
            SearchResult = x
        }).ToList();
    }
    
    public static List<PluginSearchResult> Sort(this List<PluginSearchResult> input, string searchString)
    {
        var sortValues = input.Select(x => 
            (x, StringMatcher.FuzzySearch(searchString, x.SearchResult.Title).Score)).ToList();
        
        sortValues.Sort((x, y) =>
        {
            if (x.Item2 > y.Item2)
                return -1;
            return x.Item2 < y.Item2 ? 1 : 0;
        });

        return sortValues.Select(x => x.x).ToList();
    }
}