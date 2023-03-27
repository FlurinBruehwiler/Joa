namespace Joa;

public class PluginManifest
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public string? Author { get; init; }
    public string? AuthorUrl { get; init; }
    public string? Description { get; init; }
    public string? Version { get; init; }
}