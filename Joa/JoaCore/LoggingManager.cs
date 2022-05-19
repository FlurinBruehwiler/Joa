using Interfaces.Logger;

namespace JoaCore;

public static class LoggingManager
{
    public static JoaLogger JoaLogger = new("C:/temp/Joalog.log");
}

public class JoaLogger : IJoaLogger
{
    private readonly string _log;
    
    public JoaLogger(string filePath)
    {
        _log = filePath;
    }

    public void Log(string logMessage, IJoaLogger.LogLevel logLevel)
    {
        var level = logLevel switch
        {
            IJoaLogger.LogLevel.Warning => "Warning: ",
            IJoaLogger.LogLevel.Error => "Error: ",
            _ => "Information: "
        };

        LogMessage($"[{DateTime.Now} | {level} | {logMessage}");
    }

    private void LogMessage(string message)
    {
        File.AppendAllText(_log, message + Environment.NewLine);
    }
}