using System.Diagnostics;
using JoaPluginsPackage.Injectables;

namespace JoaCore;

public class JoaLogger : IJoaLogger
{
    private static JoaLogger? _instance;

    private const string Directory = "C:/temp";
    private const string FileName = "Joalog.log";
    private readonly string _completePath;

    private JoaLogger()
    {
        _completePath = Path.Combine(Directory, FileName);
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

    public Stopwatch StartMeasure()
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        return stopwatch;
    }
    
    public void LogMeasureResult(Stopwatch stopwatch, string logName)
    {
        stopwatch.Stop();
        var time = stopwatch.ElapsedMilliseconds;
        Log($"The Execution of \"{logName}\" took {time} ms to complete", IJoaLogger.LogLevel.Info);
    }

    private void LogMessage(string message)
    {
        try
        {
            if (!File.Exists(_completePath))
            {
                System.IO.Directory.CreateDirectory(Directory);
                File.Create(FileName).Dispose();
            }

            File.AppendAllText(_completePath, message + Environment.NewLine);
        }
        catch
        {
            // ignored
        }
    }
}