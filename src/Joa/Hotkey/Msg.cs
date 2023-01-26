namespace Joa.Hotkey;

public struct Msg
{
    public readonly nint Hwnd;
    public readonly uint Message;
    public readonly nint WParam;
    public readonly nint LParam;
    public readonly uint Time;
}