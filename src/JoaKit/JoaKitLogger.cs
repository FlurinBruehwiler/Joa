using Microsoft.Extensions.Logging;

namespace JoaKit;

public class JoaLogger : ILogger
{
    private const string FileName = "./Joalog.log";

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        try
        {
            if (!File.Exists(FileName))
            {
                File.Create(FileName).Dispose();
            }

            File.AppendAllText(FileName, $"{formatter(state, exception)}\n");
        }
        catch
        {
            // ignored
        }
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        throw new NotImplementedException();
    }

    public IDisposable BeginScope<TState>(TState state) where TState : notnull
    {
        throw new NotImplementedException();
    }
}