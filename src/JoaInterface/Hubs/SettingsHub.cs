using System.Text.Json;
using JoaCore;
using JoaCore.Settings;
using JoaPluginsPackage.Injectables;
using Microsoft.AspNetCore.SignalR;

namespace JoaInterface.Hubs;

public class SettingsHub : Hub
{
    private readonly SettingsManager _settingsManager;

    public SettingsHub(SettingsManager settingsManager)
    {
        _settingsManager = settingsManager;
    }

    public void GetSettings()
    {
        JoaLogger.GetInstance().Log(JsonSerializer.Serialize(_settingsManager), IJoaLogger.LogLevel.Info);
        //await Clients.Caller.SendAsync("ReceiveSettings",  _settingsManager);
    }

    public void SetSettings()
    {
        
    }
}