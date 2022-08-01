using JoaPluginsPackage;
using JoaPluginsPackage.Plugin;

namespace JoaCore;

public class PluginCommand
{
    public PluginCommand(SearchResult searchResult, Guid pluginId)
    {
        SearchResult = searchResult;
        PluginId = pluginId;
        CommandId = Guid.NewGuid();
    }

    public SearchResult SearchResult { get; }
    public Guid PluginId { get; }
    public Guid CommandId { get; set; }
}