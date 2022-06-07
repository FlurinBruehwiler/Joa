using JoaCore;
using JoaCore.Settings;
using Microsoft.AspNetCore.SignalR;

namespace JoaInterface.Hubs;

public class SettingsHub : Hub
{
    private readonly SettingsManager _settingsManager;

    public SettingsHub(SettingsManager settingsManager)
    {
        _settingsManager = settingsManager;
    }

    public DtoSettings GetSettings()
    {
        return new DtoSettings(_settingsManager);
    }

    public void SetSettings()
    {
        
    }
}