using System.Runtime.InteropServices;
using Joa.Injectables;

namespace Joa.Hotkey;

public class HotKeyHelper
{
    private const uint WmHotkey = 0x312;
    private const int HotKeyId = 42;

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool RegisterHotKey(nint hWnd, int id, uint fsModifiers, uint virtualKeyCode);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UnregisterHotKey(nint hWnd, int id);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern int GetMessage(out Msg lpMsg, nint hWnd, uint wMsgFilterMin,
        uint wMsgFilterMax);

    public static void RegisterHotKey(Action callback, Key key, params Modifier[] modifierList)
    {
        var modifiers = modifierList.Aggregate<Modifier, uint>(0, (current, modifier) => current | (uint) modifier);

        Task.Run(() => ListenForHotKey(new HotKeyThreadParameter(callback, modifiers, (uint)key)));
    }

    private static void ListenForHotKey(HotKeyThreadParameter parameter)
    {
        if (!RegisterHotKey(nint.Zero, HotKeyId, parameter.Modifiers, parameter.Key))
        {
            JoaLogger.GetInstance().Info("Error while registering hotkeys");
            return;
        }
        JoaLogger.GetInstance().Info("Registered Hotkey");

        int status;
        while ((status = GetMessage(out var msg, nint.Zero, 0, 0)) != 0)
        {
            if (status == -1)
            {
                JoaLogger.GetInstance().Info("Error while getting Hotkey message");
                continue;
            }

            if (msg.Message != WmHotkey)
                continue;

            if((int)msg.WParam != HotKeyId)
                continue;

            JoaLogger.GetInstance().Info("Received Hotkey");

            parameter.Callback();
        }
        JoaLogger.GetInstance().Info("Stop listening for Hotkeys");
    }
}