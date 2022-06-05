namespace JoaInterface.HotKey;

public struct Msg
{
    public readonly IntPtr Hwnd;
    public readonly uint Message;
    public readonly IntPtr WParam;
    public readonly IntPtr LParam;
    public readonly uint Time;
}