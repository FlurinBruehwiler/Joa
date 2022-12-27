using System.Text.Json;
using Joa.PluginCore;
using JoaLauncher.Api;
using JoaLauncher.Api.Injectables;
using Microsoft.Extensions.Options;

namespace Joa.Settings;

public class SettingsManager
{
    private readonly PluginManager _pluginManager;
    private readonly IJoaLogger _logger;
    private readonly FileSystemManager _fileSystemManager;
    private readonly JsonSerializerOptions _options;
    private readonly FileWatcher _fileWatcher;
    
    public SettingsManager(PluginManager pluginManager, IJoaLogger logger, FileSystemManager fileSystemManager)
    {
        _pluginManager = pluginManager;
        _logger = logger;
        _fileSystemManager = fileSystemManager;
        _options = new JsonSerializerOptions
        {
            WriteIndented = true
        };
        _fileWatcher = new FileWatcher(fileSystemManager.GetSettingsLocation() ,Sync);
        Sync();
    }
    
    private void Sync()
    {
        _logger.Log("Synchronizing the settings.", LogLevel.Info);
        UpdateSettingsFromJson();
        SaveSettingsToJson();
    }
    
    private void SaveSettingsToJson()
    {
        _fileWatcher.Disable();
        using var _ = _logger.TimedOperation(nameof(SaveSettingsToJson));

        try
        {
            var dtoSetting = new DtoSettings(_pluginManager.Plugins);
            var jsonString = JsonSerializer.Serialize(dtoSetting, _options);
            File.WriteAllText(_fileSystemManager.GetSettingsLocation(), jsonString);
        }
        catch (Exception e)
        {
            _logger.Log(
                $"There was an exception thrown while Saving the Settings with the following exception{Environment.NewLine}{e}",
                LogLevel.Error);
        }
        finally
        {
            _fileWatcher.Enable();
        }
    }

    private void UpdateSettingsFromJson()
    {
        using var _ = _logger.TimedOperation(nameof(UpdateSettingsFromJson));

        try
        {
            var jsonString = File.ReadAllText(_fileSystemManager.GetSettingsLocation());
            if (string.IsNullOrEmpty(jsonString))
                return;
            var result = JsonSerializer.Deserialize<DtoSettings>(jsonString);
            if (result is null)
                throw new JsonException();

            foreach (var pluginDefinition in _pluginManager.Plugins)
            {
                UpdatePluginDefinition(pluginDefinition, result);
            }
        }
        catch (Exception e)
        {
            _logger.Log(
                $"There was an exception thrown while Updating the Settings from the settings.json with the following exception{Environment.NewLine}{e}",
                LogLevel.Error);
        }
    }

    private void UpdatePluginDefinition(PluginDefinition pluginDefinition, DtoSettings newDtoSettings)
    {
        if (!newDtoSettings.Plugins.TryGetValue(pluginDefinition.PluginInfo.Name, out var newPlugin))
            return;

        var x = newPlugin.Setting.Deserialize(pluginDefinition.Setting.GetType());

        if (x is null)
        {
            _logger.Error("Something went wrong while reading the settings");
            return;
        }

        pluginDefinition.Setting.Value = (ISetting)x;
    }
}