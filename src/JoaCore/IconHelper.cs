using JoaPluginsPackage.Injectables;

namespace JoaCore;

public class IconHelper : IIconHelper
{
    public string GetIconsDirectory(Type pluginType)
    {
        var iconsLocation = GetIconsDirectoryIfExists(pluginType);
        
        if (!Directory.Exists(iconsLocation))
            Directory.CreateDirectory(iconsLocation);

        return iconsLocation;
    }

    public string? GetIconsDirectoryIfExists(Type pluginType)
    {
        var pluginDirectory = Path.GetDirectoryName(pluginType.Assembly.Location);

        if(pluginDirectory is null)
            throw new Exception(); //ToDo
        
        var iconsLocation = Path.Combine(pluginDirectory, "Icons");

        return Directory.Exists(iconsLocation) ? iconsLocation : null;
    }
}