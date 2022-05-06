namespace Interfaces.Settings;

public class SettingText : Setting
{
    public SettingText(string name, string key, string state) : base(key, name)
    {
        State = state;
        Key = key;
    }

    public string State { get; set; }
}