namespace JoaPluginsPackage;

public class ContextAction
{
    public ContextAction(string key, string name, IShortcut shortcut, string? link = null)
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