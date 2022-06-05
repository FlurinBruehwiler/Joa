using System.Diagnostics;
using System.Runtime.InteropServices;
using JoaCore;
using JoaPluginsPackage.Logger;

namespace JoaInterface.HotKey;

public static class HotKeyHelper
{
    private const uint WmHotkey = 0x312;
    private const int HotKeyId = 42;
    
    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint virtualKeyCode);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern int GetMessage(out Msg lpMsg, IntPtr hWnd, uint wMsgFilterMin,
        uint wMsgFilterMax);
    
    public static void RegisterHotKey(Action callback, Key key, params Modifier[] modifierList)
    {
        var modifiers = modifierList.Aggregate<Modifier, uint>(0, (current, modifier) => current | (uint) modifier);

        var hotKeyListenerThread = new Thread(ListenForHotKey)
        {
            IsBackground = true
        };
        hotKeyListenerThread.Start(new HotKeyThreadParameter(callback, modifiers, (uint)key));
    }
    
    private static void ListenForHotKey(object? parameterObject)
    {
        if(parameterObject is not HotKeyThreadParameter parameter)
            return;
        
        if (!RegisterHotKey(IntPtr.Zero, HotKeyId, parameter.Modifiers, parameter.Key))
        {
            JoaLogger.GetInstance().Log("Error while registering hotkeys", IJoaLogger.LogLevel.Error);
            return;
        }
        JoaLogger.GetInstance().Log("Registered Hotkey", IJoaLogger.LogLevel.Info);
        
        int status;
        while ((status = GetMessage(out var msg, IntPtr.Zero, 0, 0)) != 0)
        {
            if (status == -1)
            {
                JoaLogger.GetInstance().Log("Error while getting Hotkey message", IJoaLogger.LogLevel.Error);
                continue;
            }

            if (msg.Message != WmHotkey)
                continue;

            if((int)msg.WParam != HotKeyId)
                continue;

            JoaLogger.GetInstance().Log("Received Hotkey", IJoaLogger.LogLevel.Info);

            parameter.Callback();
        }
        JoaLogger.GetInstance().Log("Stop listening for Hotkeys", IJoaLogger.LogLevel.Info);
    }
}