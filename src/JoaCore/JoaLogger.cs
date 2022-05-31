using System.Diagnostics;
using Interfaces.Logger;

namespace JoaCore;

public class JoaLogger : IJoaLogger
{
    private static JoaLogger? _instance;
    
    private readonly string _directory = "C:/temp";
    private readonly string _fileName = "Joalog.log";
    private readonly string _completePath;

    private JoaLogger()
    {
        _completePath = Path.Combine(_directory, _fileName);
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
        if (!File.Exists(_completePath))
        {
            Directory.CreateDirectory(_directory);
            File.Create(_fileName);
        }

        File.AppendAllText(_completePath, message + Environment.NewLine);
    }
}