using System.Security.Cryptography;

namespace Interfaces.Settings;

public class SettingToggle : Setting
{
    public SettingToggle(string name, bool state)
    {
        Name = name;
        State = state;
    }

    public sealed override string Name { get; set; }
    public bool State { get; set; }
}