using System.Security.Cryptography;

namespace Interfaces.Settings;

public class SettingToggle : Setting
{
    public SettingToggle(string name, string key , bool state)
    {
        Name = name;
        State = state;
        Key = key;
    }

    public bool State { get; set; }
}