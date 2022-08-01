using JoaPluginsPackage.Plugin;
using JoaPluginsPackage.Settings;

namespace BookmarksSearch;

public class ContextAction : IContextAction
{
    public ContextAction(string key, string name, IShortcut shortcut, string? link)
    {
        Key = key;
        Name = name;
        Shortcut = shortcut;
        Link = link;
    }

    public string Key { get; set; }
    public string Name { get; set; }
    public IShortcut Shortcut { get; set; }
    public string? Link { get; set; }
}