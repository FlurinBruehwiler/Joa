namespace JoaPluginsPackage.Settings.Attributes;

public class PluginAttribute : Attribute
{
    public PluginAttribute(string name, string description)
    {
        Name = name;
        Description = description;
    }

    public string Name { get; set; }
    public string Description { get; set; }
}