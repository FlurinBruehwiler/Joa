namespace JoaLauncher.Api.Injectables;

/// <summary>
/// Helper to help with icons
/// </summary>
public interface IIconHelper
{
    /// <summary>
    /// Gets the directory meant to store icons
    /// </summary>
    /// <param name="pluginType"></param>
    /// <returns></returns>
    public string GetIconsDirectory(Type pluginType);

    /// <summary>
    /// Stores the icon of a file
    /// </summary>
    /// <param name="fileLocation"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public string CreateIconFromFileIfNotExists<T>(string fileLocation);
}