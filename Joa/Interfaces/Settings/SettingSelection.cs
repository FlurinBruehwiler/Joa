namespace Interfaces.Settings;

public class SettingSelection<T> : Setting where T : Enum
{
    public SettingSelection(string name, string key, T state) : base(key, name)
    {
        State = state;
        Key = key;
    }

    public T State { get; set; }
}