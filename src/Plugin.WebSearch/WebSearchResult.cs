using JoaPluginsPackage;
using JoaPluginsPackage.Plugin;

namespace WebSearch;

public record WebSearchResult : SearchResult
{
    public string Url { get; init; } = default!;
}