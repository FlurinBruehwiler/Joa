using JoaLauncher.Api;

namespace Joa.PluginCore;

public record PluginSearchResult
{
    public ISearchResult SearchResult { get; init; } = default!;
    public Guid CommandId { get; } = Guid.NewGuid();
}