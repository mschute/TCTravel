using System.Runtime.CompilerServices;

namespace TCTravel.Helpers;

public static class LoggerExtensions
{
    // https://code-maze.com/csharp-how-to-find-caller-method/#:~:text=To%20get%20it%2C%20we%20use,log%20them%20to%20the%20console.
    public static void LogInformationEx(this ILogger logger, string text, [CallerMemberName] string callerMethodName = "")
    {
        logger.LogInformation($"{DateTimeOffset.UtcNow:g} | Info | {callerMethodName} | {text}");
    }
    
    public static void LogWarningEx(this ILogger logger, string text, [CallerMemberName] string callerMethodName = "")
    {
        logger.LogWarning($"{DateTimeOffset.UtcNow:g} | Warning | {callerMethodName} | {text}");
    }
    
    public static void LogErrorEx(this ILogger logger, string text, [CallerMemberName] string callerMethodName = "")
    {
        logger.LogError($"{DateTimeOffset.UtcNow:g} | Error | {callerMethodName} | {text}");
    }
}