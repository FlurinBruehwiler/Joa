namespace JoaLauncher.Api.Plugin;

/// <summary>
/// Represents a Plugin
/// </summary>
public interface IPlugin
{
    /// <summary>
    /// Is used to configure the plugin
    /// </summary>
    /// <param name="builder"></param>
    public void ConfigurePlugin(IPluginBuilder builder);
}