using Joa.Hotkey;

namespace Joa.Settings;

public class GeneralSettings
{
    public bool Autostart { get; set; }
    public Modifier HotKeyModifier1 { get; set; }
    public Modifier HotKeyModifier2 { get; set; }
    public Key HotKeyKey { get; set; }
    public int UpdateIndexInterval { get; set; }
}