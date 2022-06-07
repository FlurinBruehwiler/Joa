using System.Runtime.InteropServices;

namespace JoaInterface.MousePosition;

[StructLayout(LayoutKind.Sequential)]
public struct Point
{
    public int X;
    public int Y;

    public static implicit operator System.Drawing.Point(Point point)
    {
        return new System.Drawing.Point(point.X, point.Y);
    }
}
