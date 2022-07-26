namespace JoaPluginsPackage.Plugin;

public interface IPlugin
{
    public string Name { get; }
    public string Description { get; }
    public string Version { get; }
    public string Author { get; }
    public string SourceCode { get; }
}