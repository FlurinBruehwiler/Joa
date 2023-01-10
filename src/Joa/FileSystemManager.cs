using Microsoft.Extensions.Options;

namespace Joa;

public class FileSystemManager
{
    private readonly IOptions<PathsConfiguration> _config;

    public FileSystemManager(IOptions<PathsConfiguration> config)
    {
        _config = config;
    }

    public string GetPluginsLocation()
    {
        return CreateDirectoryIfNotExists(_config.Value.PluginLocation);
    }

    public string GetPluginsFinalLocation()
    {
        return CreateDirectoryIfNotExists(_config.Value.PluginsFinalLocation);
    }

    public string GetSettingsLocation()
    {
        return CreateFileIfNotExists(_config.Value.SettingsLocation);
    }

    private string CreateDirectoryIfNotExists(string path)
    {
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        return path;
    }

    private string CreateFileIfNotExists(string path)
    {
        if (!File.Exists(path))
            File.Create(path).Dispose();

        return path;
    }
}