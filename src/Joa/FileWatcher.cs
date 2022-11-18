using System.Diagnostics;

namespace JoaInterface;

public class FileWatcher
{
    private readonly Action _callback;
    private readonly int _delay;
    private readonly Stopwatch _swSinceLastChanged;
    private bool _isDisabled;
    private int _numberOfQuedUpChanges;
    
    public FileWatcher(string path, Action callback, int delay = 0)
    {
        _isDisabled = false;
        _callback = callback;
        _delay = delay;
        _swSinceLastChanged = Stopwatch.StartNew();
        
        var directory = path;
        string? file = null;

        if (!IsDirectory(path))
        {
            directory = Directory.GetParent(path)?.FullName ?? throw new Exception();
            file = path;
        }
        
        var watcher = new FileSystemWatcher(directory);
        watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.CreationTime;
        watcher.Changed += OnChanged;
        
        if(file is not null)
            watcher.Filter = Path.GetFileName(file);
        
        watcher.EnableRaisingEvents = true;
    }

    private bool IsDirectory(string path)
    {
        return (File.GetAttributes(path) & FileAttributes.Directory) == FileAttributes.Directory;
    }
    
    public void Disable()
    {
        _isDisabled = true;
    }

    public void Enable(int delay = 0)
    {
        Task.Delay(new TimeSpan(0, 0, 0, 0, delay)).ContinueWith(_ =>
        {
            _isDisabled = false;
        });
    }
    
    private void OnChanged(object sender, FileSystemEventArgs e)
    {
        var timeSinceLastChange = _swSinceLastChanged.ElapsedMilliseconds;
        _swSinceLastChanged.Restart();
        if (timeSinceLastChange < 100)
            return;

        if (_isDisabled)
            return;

        _numberOfQuedUpChanges++;
        
        Task.Delay(new TimeSpan(0, 0, 0, 0, _delay)).ContinueWith(_ =>
        {
            _numberOfQuedUpChanges--;
            if(_numberOfQuedUpChanges == 0)
                _callback();
        });
    }
}