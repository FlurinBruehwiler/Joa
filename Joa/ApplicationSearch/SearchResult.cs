using Interfaces;

namespace ApplicationSearch;

public class SearchResult : ISearchResult
{
    public string Caption { get; set; }
    public string Description { get; set; }
    public string Icon { get; set; }
}