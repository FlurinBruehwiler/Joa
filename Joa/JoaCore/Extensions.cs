using System.Diagnostics;
using Interfaces.Logger;

namespace JoaCore;

public static  class Extensions
{
    public static void LogMeasureResult(this Stopwatch stopwatch, string logName)
    {
        stopwatch.Stop();
        var time = stopwatch.ElapsedMilliseconds;
        LoggingManager.JoaLogger.Log($"The Execution of \"{logName}\" took {time} ms to complete", IJoaLogger.LogLevel.Info);
    }
}