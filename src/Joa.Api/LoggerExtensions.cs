using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace JoaLauncher.Api;

public static class LoggerExtensions
{
    public static IDisposable TimedLogOperation(this ILogger logger, [CallerMemberName] string nameOfMethod = "")
    {
        return new TimedLogOperation(logger, nameOfMethod);
    }
}