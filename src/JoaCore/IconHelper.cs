using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using JoaPluginsPackage.Injectables;

namespace JoaCore;

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
    public void CreateIconFromFileIfNotExists(string iconLocation, string fileLocation)
    {
        if (File.Exists(iconLocation)) 
            return;
        
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) 
            return;
        
        var icon = Icon.ExtractAssociatedIcon(fileLocation);

        if (icon is null) 
            return;

        var bitmapIcon = icon.ToBitmap();
        
        bitmapIcon.Save(iconLocation, ImageFormat.Png); 
    }
}