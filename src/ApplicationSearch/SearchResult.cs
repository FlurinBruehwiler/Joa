using Interfaces;

namespace ApplicationSearch;

public class SearchResult : ISearchResult
{
    public string Caption { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Icon { get; set; } = null!;
    public string FilePath { get; set; }
}