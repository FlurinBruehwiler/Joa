namespace JoaPluginsPackage.Attributes;

public class PluginAttribute : Attribute
{
    public PluginAttribute(string name, string description, string version, string author, string github)
    {
        Name = name;
        Description = description;
        Version = version;
        Author = author;
        Github = github;
    }
    
    public string Name { get; set; }
    public string Description { get; }
    public string Version { get; }
    public string Author { get; }
    public string Github { get; }
}