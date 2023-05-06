using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text;
using Joa.Hotkey;
using JoaLauncher.Api.Injectables;
using Microsoft.Extensions.Logging;

namespace Joa.Injectables;

public class IconHelper : IIconHelper
{
    private readonly ILogger<IconHelper> _logger;
    private readonly Bitmap _defaultIcon = new("Assets/defaultIcon.bmp");

    public IconHelper(ILogger<IconHelper> logger)
    {
        _logger = logger;
    }

    public string GetIconsDirectory(Type pluginType)
    {
        var pluginDirectory = Path.GetDirectoryName(pluginType.Assembly.Location);

        if (pluginDirectory is null)
            throw new Exception(); //ToDo

        var iconsLocation = Path.Combine(pluginDirectory, "Icons");

        if (!Directory.Exists(iconsLocation))
            Directory.CreateDirectory(iconsLocation);

        return iconsLocation;
    }

    public string CreateIconFromFileIfNotExists<T>(string fileLocation)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            throw new NotImplementedException();

        var originalLocation = fileLocation;

        if (fileLocation.EndsWith(".lnk"))
            fileLocation = GetShortcutTargetFile(fileLocation);

        if (!File.Exists(fileLocation))
            return string.Empty;

        var iconName = fileLocation.GetHashCode() + ".png";

        var iconLocation = Path
            .Combine(GetIconsDirectory(typeof(T)), iconName);

        if (File.Exists(iconLocation))
            return iconLocation;

        var icon = Icon.ExtractAssociatedIcon(fileLocation);

        if (icon is null)
            throw new Exception($"Extraction of Icon for the following file failed: {fileLocation}");

        var bitmapIcon = icon.ToBitmap();

        if (IsDefaultIcon(bitmapIcon))
        {
            var originalIcon = Icon.ExtractAssociatedIcon(originalLocation);

            if (originalIcon is null)
                throw new Exception($"Extraction of Icon for the following file failed: {originalLocation}");

            bitmapIcon = originalIcon.ToBitmap();
        }

        bitmapIcon.Save(iconLocation, ImageFormat.Png);

        return iconLocation;
    }

    [DllImport("msvcrt.dll")]
    private static extern int memcmp(nint b1, nint b2, long count);

    private bool IsDefaultIcon(Bitmap bitmap)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            throw new NotImplementedException();

        if (bitmap.Size != _defaultIcon.Size) return false;

        var bd1 = bitmap.LockBits(new Rectangle(new Point(0, 0), bitmap.Size), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
        var bd2 = _defaultIcon.LockBits(new Rectangle(new Point(0, 0), _defaultIcon.Size), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

        try
        {
            var bd1Scan0 = bd1.Scan0;
            var bd2Scan0 = bd2.Scan0;

            var stride = bd1.Stride;
            var len = stride * bitmap.Height;

            return memcmp(bd1Scan0, bd2Scan0, len) == 0;
        }
        finally
        {
            bitmap.UnlockBits(bd1);
            _defaultIcon.UnlockBits(bd2);
        }
    }

    private string GetShortcutTargetFile(string file)
    {
        try
        {
            var link = new ShellLink();
            // ReSharper disable once SuspiciousTypeConversion.Global
            ((IPersistFile)link).Load(file, External.STGM_READ);
            var sb = new StringBuilder(External.MAX_PATH);
            // ReSharper disable once SuspiciousTypeConversion.Global
            ((IShellLinkW)link).GetPath(sb, sb.Capacity, out _, 0);

            return sb.ToString();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Cant get shortcut target for: {file}", file);
            return file;
        }
    }
}