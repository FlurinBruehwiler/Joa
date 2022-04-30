namespace Interfaces.Settings;

public class SettingList : Setting
{
    public SettingList(string name, string key, List<Setting> state, string addText)
    {
        Name = name;
        State = state;
        AddText = addText;
        Key = key;
    }

    public List<Setting> State { get; set; }
    public string AddText { get; set; }
}