using System.Runtime.InteropServices;
using Joa.Hotkey;
using JoaLauncher.Api.Injectables;
using Microsoft.Extensions.Logging;

namespace Joa.Injectables;

public class ClipboardHelper : IClipboardHelper
{
    private readonly ILogger<ClipboardHelper> _logger;

    public ClipboardHelper(ILogger<ClipboardHelper> logger)
    {
        _logger = logger;
    }

    public void Copy(string text)
    {
        if (!External.OpenClipboard(nint.Zero))
        {
            _logger.LogError("Failed to open clipboard.");
            return;
        }

        External.EmptyClipboard();

        var hMem = Marshal.StringToHGlobalUni(text);

        if (External.SetClipboardData(13, hMem) == nint.Zero)
        {
            _logger.LogError("Failed to set clipboard data.");
        }

        External.CloseClipboard();
    }
}