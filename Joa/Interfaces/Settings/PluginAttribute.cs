namespace Interfaces.Settings;

public class PluginAttribute : Attribute
{
    public PluginAttribute(string name, string description)
    {
        Name = name;
        Description = description;
    }

    private string Name { get; set; }
    private string Description { get; set; }
}