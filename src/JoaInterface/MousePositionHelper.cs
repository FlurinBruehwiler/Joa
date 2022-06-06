using System.Drawing;
using System.Runtime.InteropServices;

namespace JoaInterface;

public static class MousePositionHelper
{
    [DllImport("user32.dll")]
    private static extern bool GetCursorPos(out Point lpPoint);
    
    public static Point GetMousePosition()
    {
        GetCursorPos(out var point);
        return point;
    }
}

