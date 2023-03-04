using Joa.Hotkey;

namespace Joa.Settings;

public class GeneralSettings
{
    public bool Autostart { get; set; }
    public Modifier HotKeyModifier1 { get; set; } = Modifier.Alt;
    public Modifier HotKeyModifier2 { get; set; } = Modifier.None;
    public Key HotKeyKey { get; set; } = Key.P;
    public int UpdateIndexInterval { get; set; }
}