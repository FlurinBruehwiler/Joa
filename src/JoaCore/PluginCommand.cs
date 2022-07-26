using JoaPluginsPackage.Plugin;

namespace JoaCore;

public class PluginCommand
{
    public PluginCommand(ISearchResult searchResult, Guid pluginId)
    {
        SearchResult = searchResult;
        PluginId = pluginId;
        CommandId = Guid.NewGuid();
    }

    public ISearchResult SearchResult { get; }
    public Guid PluginId { get; }
    public Guid CommandId { get; set; }
}