using JoaLauncher.Api;

namespace JoaInterface.PluginCore;

public record PluginSearchResult
{
    public ISearchResult SearchResult { get; init; } = default!;
    public Guid CommandId { get; } = Guid.NewGuid();
}