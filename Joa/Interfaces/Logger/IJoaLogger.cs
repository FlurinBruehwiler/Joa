using System.Diagnostics;

namespace Interfaces.Logger;

public interface IJoaLogger
{
    public enum LogLevel
    {
        Warning,
        Info,
        Error
    }

    public void Log(string message, LogLevel logLevel);

    public Stopwatch StartMeasure();
    public void LogMeasureResult(Stopwatch stopwatch, string logName);
}