using JoaPluginsPackage.Settings;

namespace JoaPluginsPackage.Plugin;

public interface IAction
{
    public string Key { get; set; }
    public string Name { get; set; }
    public IShortcut Shortcut { get; set; }
    public string? Link { get; set; }
}