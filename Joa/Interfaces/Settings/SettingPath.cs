namespace Interfaces.Settings;

public class SettingPath : Setting
{
    public SettingPath(string name, string path)
    {
        Name = name;
        Path = path;
    }

    public sealed override string Name { get; set; }
    public string Path { get; set; }
}