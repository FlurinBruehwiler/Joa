using JoaLauncher.Api.Injectables;

namespace Joa.Hotkey;

public class HotKeyService : IDisposable
{
    private readonly IJoaLogger _joaLogger;
    private readonly Dictionary<int, HotKey> _registerdHotkeys = new();
    private readonly Dictionary<int, HotKey> _hotKeysToRegister = new();
    private readonly List<int> _hotKeysToUnregister = new();
    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly CancellationToken _cancellationToken;

    public HotKeyService(IJoaLogger joaLogger)
    {
        _cancellationTokenSource = new CancellationTokenSource();
        _cancellationToken = _cancellationTokenSource.Token;
        _joaLogger = joaLogger;
        Task.Run(ListenForHotKey, _cancellationToken);
    }

    public int RegisterHotKey(Action callback, Key key, params Modifier[] modifiers)
    {
        var id = GetUniqueHotKeyId();
        _hotKeysToRegister.Add(id, new HotKey(key, modifiers, callback));
        return id;
    }

    public void UnregisterHotKey(int hotKeyId)
    {
        if (_hotKeysToRegister.Remove(hotKeyId))
            return;

        if (_registerdHotkeys.ContainsKey(hotKeyId))
            _hotKeysToUnregister.Add(hotKeyId);
    }

    private void ListenForHotKey()
    {
        while (!_cancellationToken.IsCancellationRequested)
        {
            var status = External.PeekMessageA(out var msg, nint.Zero, 0, 0, External.PM_REMOVE);

            RegisterHotkeys();
            UnregisterHotKeys();

            Thread.Sleep(1);

            if (status == 0)
                continue;

            if (status == -1)
            {
                _joaLogger.Info("Error while getting Hotkey message");
                continue;
            }

            if (msg.Message != External.WmHotkey)
                continue;

            var hotKeyId = (int)msg.WParam;

            if (!_registerdHotkeys.TryGetValue(hotKeyId, out var hotkey))
                continue;

            _joaLogger.Info($"Received Hotkey: {hotkey}");

            // ReSharper disable once MethodSupportsCancellation
            Task.Run(hotkey.Callback);
        }
        _hotKeysToUnregister.AddRange(_registerdHotkeys.Select(x => x.Key));
        UnregisterHotKeys();
        _joaLogger.Info("Stop listening for Hotkeys");
    }

    private void UnregisterHotKeys()
    {
        foreach (var id in _hotKeysToUnregister)
        {
            if (!External.UnregisterHotKey(nint.Zero, id))
            {
                _joaLogger.Error($"Failed to unregister hot key with id {id}");
                continue;
            }

            _registerdHotkeys.Remove(id);
        }
    }

    private void RegisterHotkeys()
    {
        foreach (var (id, hotKey) in _hotKeysToRegister)
        {
            var modifiers = hotKey.Modifiers.Aggregate<Modifier, uint>(0, (current, modifier) => current | (uint)modifier);

            if (!External.RegisterHotKey(nint.Zero, id, modifiers, (uint)hotKey.Key))
            {
                _joaLogger.Info($"Error while registering hotkey: {hotKey}");
                continue;
            }

            _registerdHotkeys.Add(id, hotKey);
            _joaLogger.Info($"Registered Hotkey: {hotKey}");
        }

        _hotKeysToRegister.Clear();
    }

    private int GetUniqueHotKeyId()
    {
        var rand = new Random();
        int hotKeyId;

        do
        {
            hotKeyId = rand.Next();
        } while (_registerdHotkeys.ContainsKey(hotKeyId) || _hotKeysToRegister.ContainsKey(hotKeyId));

        return hotKeyId;
    }

    public void Dispose()
    {
        _cancellationTokenSource.Cancel();
    }
}

public record HotKey(Key Key, Modifier[] Modifiers, Action Callback);