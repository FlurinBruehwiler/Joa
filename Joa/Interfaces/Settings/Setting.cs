namespace Interfaces.Settings;

public abstract class Setting
{
    protected Setting(string key, string name)
    {
        Name = name;
        Key = key;
    }

    public string Name { get; set; }
    public string Key { get; set; }
}