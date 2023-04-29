namespace JoaKit;

internal enum LogLevel
{
    Warning,
    Info,
    Error,
}

internal class JoaKitLogger
{
    private static JoaKitLogger? _instance;

    private const string FileName = "./Joalog.log";

    private JoaKitLogger()
    {
    }

    public void Log(string logMessage, LogLevel logLevel)
    {
        var level = logLevel switch
        {
            LogLevel.Warning => "Warning:",
            LogLevel.Error => "Error:",
            _ => "Information:"
        };

        LogMessage($"[{DateTime.Now} | {level} | {logMessage}");
    }

    public void Info(string logMessage)
    {
        Log(logMessage, LogLevel.Info);
    }

    public void Warning(string logMessage)
    {
        Log(logMessage, LogLevel.Warning);
    }

    public void Error(string logMessage)
    {
        Log(logMessage, LogLevel.Error);
    }

    public void LogException(Exception e, string logName = "")
    {
        Log(
            string.IsNullOrEmpty(logName)
                ? $"There was an Exception with the following Stacktrace {e}"
                : $"{logName} with the following Stacktrace {e}",
            LogLevel.Error);
    }

    public static JoaKitLogger GetInstance()
    {
        return _instance ??= new JoaKitLogger();
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