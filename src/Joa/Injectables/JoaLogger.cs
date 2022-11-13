using Joa.Api.Injectables;

namespace JoaInterface.Injectables;

public class JoaLogger : IJoaLogger
{
    private static JoaLogger? _instance;

    private const string FileName = "./Joalog.log";

    private JoaLogger()
    {
    }

    public void Log(string logMessage, IJoaLogger.LogLevel logLevel)
    {
        var level = logLevel switch
        {
            IJoaLogger.LogLevel.Warning => "Warning:",
            IJoaLogger.LogLevel.Error => "Error:",
            _ => "Information:"
        };

        LogMessage($"[{DateTime.Now} | {level} | {logMessage}");
    }

    public void Info(string logMessage)
    {
        Log(logMessage, IJoaLogger.LogLevel.Info);
    }

    public void Warning(string logMessage)
    {
        Log(logMessage, IJoaLogger.LogLevel.Warning);
    }

    public void Error(string logMessage)
    {
        Log(logMessage, IJoaLogger.LogLevel.Error);
    }

    public void LogException(Exception e, string logName = "")
    {
        Log(
            string.IsNullOrEmpty(logName)
                ? $"There was an Exception with the following Stacktrace {e}"
                : $"{logName} with the following Stacktrace {e}",
            IJoaLogger.LogLevel.Error);
    }

    public static JoaLogger GetInstance()
    {
        return _instance ??= new JoaLogger();
    }

    public IDisposable TimedOperation(string nameOfMethod)
    {
        return new TimedLogOperation(this, nameOfMethod);
    }

    private void LogMessage(string message)
    {
        try
        {
            if (!File.Exists(FileName))
            {
                File.Create(FileName).Dispose();
            }

            File.AppendAllText(FileName, message + Environment.NewLine);
        }
        catch
        {
            // ignored
        }
    }
}