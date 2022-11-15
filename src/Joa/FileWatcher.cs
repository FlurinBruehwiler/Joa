using System.Diagnostics;

namespace JoaInterface;

public class FileWatcher
{
    private readonly Action _callback;
    private readonly Stopwatch _swSinceLastChanged;
    private bool _isDisabled;
    
    public FileWatcher(string path, Action callback)
    {
        _isDisabled = false;
        _callback = callback;
        _swSinceLastChanged = Stopwatch.StartNew();
        
        var directory = path;
        string? file = null;

        if (!path.IsDirectory())
        {
            directory = Directory.GetParent(path)?.FullName ?? throw new Exception();
            file = path;
        }
        
        var watcher = new FileSystemWatcher(directory);
        watcher.NotifyFilter = NotifyFilters.LastWrite;
        watcher.Changed += OnChanged;
        
        if(file is not null)
            watcher.Filter = Path.GetFileName(file);
        
        watcher.EnableRaisingEvents = true;
    }

    public void Disable()
    {
        _isDisabled = true;
    }

    public void Enable()
    {
        _isDisabled = false;
    }
    
    private void OnChanged(object sender, FileSystemEventArgs e)
    {
        var timeSinceLastChange = _swSinceLastChanged.ElapsedMilliseconds;
        _swSinceLastChanged.Restart();
        if (timeSinceLastChange < 100)
            return;

        if (_isDisabled)
            return;
        
        _callback();
    }
}