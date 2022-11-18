using System.Text.Json;
using JoaLauncher.Api;
using JoaLauncher.Api.Injectables;
using JoaInterface.PluginCore;
using Microsoft.Extensions.Options;

namespace JoaInterface.Settings;

public class SettingsManager
{
    private readonly PluginManager _pluginManager;
    private readonly IOptions<PathsConfiguration> _configuration;
    private readonly IJoaLogger _logger;
    private readonly JsonSerializerOptions _options;
    private readonly FileWatcher _fileWatcher;
    
    public SettingsManager(PluginManager pluginManager, IOptions<PathsConfiguration> configuration, IJoaLogger logger)
    {
        logger.Info(nameof(SettingsManager));
        _pluginManager = pluginManager;
        _configuration = configuration;
        _logger = logger;
        _options = new JsonSerializerOptions
        {
            WriteIndented = true
        };
        _fileWatcher = new FileWatcher(configuration.Value.SettingsLocation, Sync);
        Sync();
    }
    
    private void Sync()
    {
        _logger.Log("Synchronizing the settings.", IJoaLogger.LogLevel.Info);
        UpdateSettingsFromJson();
        SaveSettingsToJson();
    }
    
    private void SaveSettingsToJson()
    {
        _fileWatcher.Disable();
        using var _ = _logger.TimedOperation(nameof(SaveSettingsToJson));

        try
        {
            if (!File.Exists(_configuration.Value.SettingsLocation))
            {
                File.Create(_configuration.Value.SettingsLocation).Dispose();
            }
            var dtoSetting = new DtoSettings(_pluginManager.Plugins);
            var jsonString = JsonSerializer.Serialize(dtoSetting, _options);
            File.WriteAllText(_configuration.Value.SettingsLocation, jsonString);
        }
        catch (Exception e)
        {
            _logger.Log(
                $"There was an exception thrown while Saving the Settings with the following exception{Environment.NewLine}{e}",
                IJoaLogger.LogLevel.Error);
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
            var jsonString = File.ReadAllText(_configuration.Value.SettingsLocation);
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
                IJoaLogger.LogLevel.Error);
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

        pluginDefinition.Setting = (ISetting)x;
    }
}