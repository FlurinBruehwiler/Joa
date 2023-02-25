using System.Runtime.InteropServices;

namespace Joa.Hotkey;

public static class External
{
    public const uint WmHotkey = 0x312;

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool RegisterHotKey(nint hWnd, int id, uint fsModifiers, uint virtualKeyCode);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool UnregisterHotKey(nint hWnd, int id);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern int GetMessage(out Msg lpMsg, nint hWnd, uint wMsgFilterMin,
        uint wMsgFilterMax);
}