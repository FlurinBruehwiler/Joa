using System.Diagnostics;
using JoaLauncher.Api.Injectables;

namespace Joa;

public class TimedLogOperation : IDisposable
{
    private readonly IJoaLogger _logger;
    private readonly string _message;
    private readonly Stopwatch _stopwatch;
    
    public TimedLogOperation(IJoaLogger logger, string message)
    {
        _logger = logger;
        _message = message;
        _stopwatch = Stopwatch.StartNew();
    }

    public void Dispose()
    {
        _stopwatch.Stop();
        _logger.Info($"{_message} completet in {_stopwatch.ElapsedMilliseconds}ms");
    }
}