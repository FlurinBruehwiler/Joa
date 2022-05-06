namespace Interfaces.Settings;

public class SettingPath : Setting
{
    public SettingPath(string name, string key, string path) : base(key, name)
    {
        Path = path;
        Key = key;
    }

    public string Path { get; set; }
}