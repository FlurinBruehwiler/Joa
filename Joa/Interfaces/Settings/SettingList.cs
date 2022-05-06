namespace Interfaces.Settings;

public class SettingList : Setting
{
    public SettingList(string name, string key, List<Setting> state, string addText) : base(key, name)
    {
        State = state;
        AddText = addText;
    }

    public List<Setting> State { get; set; }
    public string AddText { get; set; }
}