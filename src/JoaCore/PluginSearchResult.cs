using JoaPluginsPackage;

namespace JoaCore;

public record PluginSearchResult
{
    public ISearchResult SearchResult { get; init; } = default!;
    public Guid CommandId { get; } = Guid.NewGuid();
}