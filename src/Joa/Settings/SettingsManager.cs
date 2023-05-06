using System.Text.Json;
using Joa.BuiltInPlugin;
using Joa.PluginCore;
using JoaLauncher.Api;
using Microsoft.Extensions.Logging;

namespace Joa.Settings;

public class SettingsManager
{
    private readonly PluginManager _pluginManager;
    private readonly ILogger<SettingsManager> _logger;
    private readonly FileSystemManager _fileSystemManager;
    private readonly JsonSerializerOptions _options;
    private readonly FileWatcher _fileWatcher;

    public Action SettingsChangedOutsideOfUi { get; set; }
    public BuiltInSettings BuiltInSettings { get; set; } = new();

    public SettingsManager(PluginManager pluginManager, ILogger<SettingsManager> logger, FileSystemManager fileSystemManager)
    {
        _pluginManager = pluginManager;
        _logger = logger;
        _fileSystemManager = fileSystemManager;
        _options = new JsonSerializerOptions
        {
            WriteIndented = true
        };
        _fileWatcher = new FileWatcher(fileSystemManager.GetSettingsLocation(), () =>
        {
            Sync();
            SettingsChangedOutsideOfUi();
        }, 10);
        Sync();
        pluginManager.UpdateIndexesAsync().GetAwaiter().GetResult();
    }

    private void Sync()
    {
        _logger.LogInformation("Synchronizing the settings.");
        UpdateSettingsFromJson();
        SaveSettingsToJsonAsync().GetAwaiter().GetResult();
    }

    public async Task PropertyHasChanged(PluginDefinition pluginDefinition, string property)
    {
        var saveAction = pluginDefinition.SaveActions.FirstOrDefault(x =>
            x.Type == pluginDefinition.Setting.GetType() && x.Property == property);

        if (saveAction is null)
            return;

        if (saveAction.Callback is not null)
        {
            saveAction.Callback(pluginDefinition.Setting);
        }

        if (saveAction.AsyncCallback is not null)
        {
            await saveAction.AsyncCallback(pluginDefinition.Setting);
        }
    }

    public async Task SaveSettingsToJsonAsync()
    {
        _fileWatcher.Disable();
        using var _ = _logger.TimedLogOperation();

        // _globalHotKey.RegisterUiHotKey();

        try
        {
            var dtoSetting = new DtoSettings(_pluginManager.Plugins, BuiltInSettings);
            var jsonString = JsonSerializer.Serialize(dtoSetting, _options);
            await File.WriteAllTextAsync(_fileSystemManager.GetSettingsLocation(), jsonString);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "There was an exception thrown while Saving the Settings");
        }
        finally
        {
            _fileWatcher.Enable();
        }
    }

    private void UpdateSettingsFromJson()
    {
        using var _ = _logger.TimedLogOperation();

        try
        {
            var jsonString = File.ReadAllText(_fileSystemManager.GetSettingsLocation());
            if (string.IsNullOrEmpty(jsonString))
                return;

            var result = JsonSerializer.Deserialize<DtoSettings>(jsonString);
            if (result is null)
                throw new JsonException();

            JsonUtilities.PopulateObject(jsonString, BuiltInSettings);

            foreach (var pluginDefinition in _pluginManager.Plugins)
            {
                UpdatePluginDefinition(pluginDefinition, result);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "There was an exception thrown while Updating the Settings from the settings.json");
        }
    }

    private void UpdatePluginDefinition(PluginDefinition pluginDefinition, DtoSettings newDtoSettings)
    {
        if (!newDtoSettings.Plugins.TryGetValue(pluginDefinition.Manifest.Id, out var newPlugin))
            return;

        JsonUtilities.PopulateObject(newPlugin.Setting, pluginDefinition.Setting);
    }
}