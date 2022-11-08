namespace JoaPluginsPackage.Injectables;

public interface IJoaLogger
{
    public enum LogLevel
    {
        Warning,
        Info,
        Error
    }

    public void Log(string message, LogLevel logLevel);
    public void Info(string message);
    public void Warning(string message);
    public void Error(string message);
    public IDisposable TimedOperation(string message);
    public void LogException(Exception e, string logName = "");
}