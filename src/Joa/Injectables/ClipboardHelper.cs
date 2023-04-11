using System.Runtime.InteropServices;
using Joa.Hotkey;
using JoaLauncher.Api.Injectables;

namespace Joa.Injectables;

public class ClipboardHelper : IClipboardHelper
{
    public void Copy(string text)
    {
        if (!External.OpenClipboard(nint.Zero))
        {
            JoaLogger.GetInstance().Error("Failed to open clipboard.");
            return;
        }

        External.EmptyClipboard();

        var hMem = Marshal.StringToHGlobalUni(text);

        if (External.SetClipboardData(13, hMem) == nint.Zero)
        {
            JoaLogger.GetInstance().Error("Failed to set clipboard data.");
        }

        External.CloseClipboard();
    }
}