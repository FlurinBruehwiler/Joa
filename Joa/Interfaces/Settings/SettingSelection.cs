namespace Interfaces.Settings;

public class SettingSelection<T> : Setting where T : Enum
{
    public SettingSelection(string name, T state)
    {
        Name = name;
        State = state;
    }

    public sealed override string Name { get; set; }
    public T State { get; set; }
}