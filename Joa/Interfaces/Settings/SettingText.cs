namespace Interfaces.Settings;

public class SettingText : Setting
{
    public SettingText(string name, string state)
    {
        Name = name;
        State = state;
    }

    public sealed override string Name { get; set; }
    public string State { get; set; }
}