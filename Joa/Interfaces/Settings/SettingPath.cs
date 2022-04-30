namespace Interfaces.Settings;

public class SettingPath : Setting
{
    public SettingPath(string name, string key, string path)
    {
        Name = name;
        Path = path;
        Key = key;
    }

    public string Path { get; set; }
}