using JoaPluginsPackage;
using JoaPluginsPackage.Plugin;

namespace JoaCore;

public record PluginSearchResult
{
    public SearchResult SearchResult { get; init; } = default!;
    public Guid PluginId { get; init; }
    public Guid CommandId { get; } = Guid.NewGuid();
}