using JoaPluginsPackage;
using JoaPluginsPackage.Plugin;

namespace ApplicationSearch;

public record ApplicationSearchResult : SearchResult
{
    public string FilePath { get; init; } = default!;
}