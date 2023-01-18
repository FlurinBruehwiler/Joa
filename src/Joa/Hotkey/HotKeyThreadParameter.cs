namespace Joa.Hotkey;

public class HotKeyThreadParameter
{
    public HotKeyThreadParameter(Action callback, uint modifiers, uint key)
    {
        Callback = callback;
        Modifiers = modifiers;
        Key = key;
    }

    public Action Callback { get; set; }
    public uint Modifiers { get; set; }
    public uint Key { get; set; }
}