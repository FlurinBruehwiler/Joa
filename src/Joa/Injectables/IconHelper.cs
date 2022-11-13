using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using JoaPluginsPackage.Injectables;

namespace JoaInterface.Injectables;

public class IconHelper : IIconHelper
{
    public string GetIconsDirectory(Type pluginType)
    {
        var pluginDirectory = Path.GetDirectoryName(pluginType.Assembly.Location);

        if(pluginDirectory is null)
            throw new Exception(); //ToDo
        
        var iconsLocation = Path.Combine(pluginDirectory, "Icons");

        if (!Directory.Exists(iconsLocation))
            Directory.CreateDirectory(iconsLocation);

        return iconsLocation;
    }
    
    public string CreateIconFromFileIfNotExists<T>(string fileLocation)
    {
        if (!File.Exists(fileLocation))
            throw new Exception($"Could not find the following file: {fileLocation}");
        
        var iconLocation = Path.Combine(GetIconsDirectory(typeof(T)), Path.ChangeExtension(Path.GetFileName(fileLocation), ".png"));
        
        if (File.Exists(iconLocation)) 
            return iconLocation;

        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            throw new NotImplementedException();
        
        var icon = Icon.ExtractAssociatedIcon(fileLocation);

        if (icon is null)
            throw new Exception($"Extraction of Icon for the following file failed: {fileLocation}");

        var bitmapIcon = icon.ToBitmap();
        
        bitmapIcon.Save(iconLocation, ImageFormat.Png);

        return iconLocation;
    }
}