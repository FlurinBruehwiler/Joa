using JoaPluginsPackage;

namespace JoaCore;

public record PluginSearchResult
{
    public ISearchResult SearchResult { get; init; } = default!;
    public Guid PluginId { get; init; }
    public Guid CommandId { get; } = Guid.NewGuid();
}