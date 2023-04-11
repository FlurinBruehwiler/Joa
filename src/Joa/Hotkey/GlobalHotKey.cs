using Joa.Settings;

namespace Joa.Hotkey;

public class GlobalHotKey
{
    private readonly SettingsManager _settingsManager;
    private readonly HotKeyService _hotKeyService;
    private Action? _uiHotKeyAction;
    private Key _currentKey;
    private Modifier _currentModifier1;
    private Modifier _currentModifier2;
    private int? _currentHotKeyId;

    public GlobalHotKey(SettingsManager settingsManager, HotKeyService hotKeyService)
    {
        _settingsManager = settingsManager;
        _hotKeyService = hotKeyService;
    }

    public void InitialHotKeyRegistration(Action callback)
    {
        _uiHotKeyAction = callback;
        RegisterUiHotKey();
    }

    public void RegisterUiHotKey()
    {
        if (_uiHotKeyAction is null)
            return;

        var generalSettings = _settingsManager.BuiltInSettings;

        if (HotKeyHasChanged() || _currentHotKeyId is null)
        {
            _currentKey = generalSettings.HotKeyKey;
            _currentModifier1 = generalSettings.HotKeyModifier1;
            _currentModifier2 = generalSettings.HotKeyModifier2;

            var modifiers = new List<Modifier>();

            if (_currentModifier1 != Modifier.None)
                modifiers.Add(_currentModifier1);
            if (_currentModifier2 != Modifier.None)
                modifiers.Add(_currentModifier2);

            if (_currentHotKeyId is not null)
                _hotKeyService.UnregisterHotKey(_currentHotKeyId.Value);

            _currentHotKeyId =
                _hotKeyService.RegisterHotKey(_uiHotKeyAction, generalSettings.HotKeyKey, modifiers.ToArray());
        }
    }

    public bool HotKeyHasChanged()
    {
        var generalSettings = _settingsManager.BuiltInSettings;

        return _currentKey != generalSettings.HotKeyKey ||
               _currentModifier1 != generalSettings.HotKeyModifier1 ||
               _currentModifier2 != generalSettings.HotKeyModifier2;
    }
}