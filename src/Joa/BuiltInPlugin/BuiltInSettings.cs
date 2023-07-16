using FlurinBruehwiler.Helpers;
using JoaLauncher.Api.Plugin;

namespace Joa.BuiltInPlugin;

public class BuiltInSettings : ISetting
{
    public bool Autostart { get; set; }
    public Modifier HotKeyModifier1 { get; set; } = Modifier.Alt;
    public Modifier HotKeyModifier2 { get; set; } = Modifier.None;
    public Key HotKeyKey { get; set; } = Key.P;
    public int UpdateIndexInterval { get; set; }
}