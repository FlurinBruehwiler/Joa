using Interfaces.Logger;

namespace JoaCore;

public static class LoggingManager
{
    public static JoaLogger JoaLogger = new("C:/temp", "Joalog.log");
}

public class JoaLogger : IJoaLogger
{
    private readonly string _directory;
    private readonly string _fileName;
    private readonly string _completePath;

    public JoaLogger(string directory, string fileName)
    {
        _directory = directory;
        _fileName = fileName;
        _completePath = Path.Combine(directory, fileName);
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
        if (!File.Exists(_completePath))
        {
            Directory.CreateDirectory(_directory);
            File.Create(_fileName);
        }

        File.AppendAllText(_completePath, message + Environment.NewLine);
    }
}