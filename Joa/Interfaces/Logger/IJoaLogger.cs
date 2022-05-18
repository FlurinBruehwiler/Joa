namespace Interfaces.Logger;

public interface IJoaLogger
{
    public enum LogLevel
    {
        Warning,
        Info,
        Error
    }

    public void Log(string message, LogLevel ll);
}