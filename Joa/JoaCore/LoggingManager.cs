using Interfaces.Logger;

namespace JoaCore;

public static class LoggingManager
{
    public static JoaLogger JoaLogger = new JoaLogger("C:/temp/Joalog.txt", "JoaLogger");
}

public class JoaLogger : IJoaLogger
{
    private string _me;
    private readonly string _log;
    
    public JoaLogger(string filePath, string name)
    {
        _me = name;
        _log = filePath;
    }

    public void Log(string logMessage, IJoaLogger.LogLevel ll)
    {
        var prefix = "Information:";
        
        switch (ll)
        {
            case IJoaLogger.LogLevel.Warning:
                prefix = "Warning: ";
                break;
            case IJoaLogger.LogLevel.Error:
                prefix = "Error: ";
                break;
            default:
                prefix = "Information: ";
                break;
        }
        
        LogMessage(prefix + logMessage);
    }

    private void LogMessage(string message)
    {
        using (var w = File.AppendText(_log))
        {
            w.WriteLine(message);
        }
    }
    
}