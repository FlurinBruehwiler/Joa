using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using JoaLauncher.Api.Injectables;

// using Shell32;

namespace Joa.Injectables;

public class IconHelper : IIconHelper
{
    public string GetIconsDirectory(Type pluginType)
    {
        var iconsLocation = Path.Combine("wwwroot", "Icons");

        if (!Directory.Exists(iconsLocation))
            Directory.CreateDirectory(iconsLocation);

        return iconsLocation;
    }

    public string CreateIconFromFileIfNotExists<T>(string fileLocation)
    {
        // throw new Exception($"Could not find the following file: {fileLocation}");
        
        if (fileLocation.EndsWith(".lnk"))
            fileLocation = GetShortcutTargetFile(fileLocation);

        if (!File.Exists(fileLocation))
            return string.Empty;
        
        var iconLocation = Path
            .Combine(GetIconsDirectory(typeof(T)), Path.ChangeExtension(Path.GetFileName(fileLocation), ".png"))
            .Replace(" ", "%20");

        if (File.Exists(iconLocation))
            return iconLocation;

        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            throw new NotImplementedException();

        var icon = Icon.ExtractAssociatedIcon(fileLocation);

        if (icon is null)
            throw new Exception($"Extraction of Icon for the following file failed: {fileLocation}");

        var bitmapIcon = icon.ToBitmap();

        bitmapIcon.Save(iconLocation, ImageFormat.Png);

        return Path.GetFileName(iconLocation);
    }

    private string GetShortcutTargetFile(string file)
    {
        try
        {
            var pathOnly = Path.GetDirectoryName(file);
            var filenameOnly = Path.GetFileName(file);

            dynamic shell = Activator.CreateInstance(Type.GetTypeFromProgID("shell.application", true));
            var folder = shell.NameSpace(pathOnly);
            var folderItem = folder.ParseName(filenameOnly);
            if (folderItem != null)
            {
                return folderItem.GetLink.Path;
            }

            return string.Empty;
        }
        catch (Exception e)
        {
            JoaLogger.GetInstance().Error(e.Message);
        }

        return string.Empty; 
    }
}