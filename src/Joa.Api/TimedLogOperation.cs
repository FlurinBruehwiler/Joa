using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace JoaLauncher.Api;

internal class TimedLogOperation : IDisposable
{
    private readonly ILogger _logger;
    private readonly string _message;
    private readonly Stopwatch _stopwatch;

    public TimedLogOperation(ILogger logger, string message)
    {
        _logger = logger;
        _message = message;
        _stopwatch = Stopwatch.StartNew();
    }

    public void Dispose()
    {
        _stopwatch.Stop();
        _logger.LogInformation($"{_message} completet in {_stopwatch.ElapsedMilliseconds}ms");
    }
}