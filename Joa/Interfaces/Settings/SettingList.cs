namespace Interfaces.Settings;

public class SettingList : Setting
{
    public SettingList(string name, List<Setting> state, string addText)
    {
        Name = name;
        State = state;
        AddText = addText;
    }

    public sealed override string Name { get; set; }
    public List<Setting> State { get; set; }
    public string AddText { get; set; }
}