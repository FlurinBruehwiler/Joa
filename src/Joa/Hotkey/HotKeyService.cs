using Joa.Settings;

namespace Joa.Hotkey;

public class HotKeyService
{
    private readonly SettingsManager _settingsManager;
    private Action? _uiHotKeyAction;
    private Key _currentKey;
    private Modifier _currentModifier1;
    private Modifier _currentModifier2;
    private int _currentHotKeyId;

    public HotKeyService(SettingsManager settingsManager)
    {
        _settingsManager = settingsManager;
    }
    
    public void InitialHotKeyRegistration(Action callback)
    {
        _uiHotKeyAction = callback;
        RegisterUiHotKey();
    }

    public void RegisterUiHotKey()
    {
        var generalSettings = _settingsManager.GeneralSettings;
        
        if ((_currentKey != generalSettings.HotKeyKey ||
             _currentModifier1 != generalSettings.HotKeyModifier1 ||
             _currentModifier2 != generalSettings.HotKeyModifier2) && _uiHotKeyAction is not null)
        {
            _currentKey = generalSettings.HotKeyKey;
            _currentModifier1 = generalSettings.HotKeyModifier1;
            _currentModifier2 = generalSettings.HotKeyModifier2;

            var modifiers = new List<Modifier>();
            
            if(_currentModifier1 != Modifier.None)
                modifiers.Add(_currentModifier1);
            if(_currentModifier2 != Modifier.None)
                modifiers.Add(_currentModifier2);
            
            
        }
    }
}