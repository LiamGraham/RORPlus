using BepInEx.Logging;
using System.Runtime.CompilerServices;

namespace RORPlus
{
    // Based on MSULog from https://github.com/TeamMoonstorm/MoonstormSharedUtils
    internal class RLogger
    {
        internal static ManualLogSource logger = null;

        internal static void LogDebug(object data, [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string member = "")
        {
            logger.LogDebug(LogString(data, lineNumber, member));
        }
        internal static void LogError(object data, [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string member = "")
        {
            logger.LogError(LogString(data, lineNumber, member));
        }
        internal static void LogFatal(object data, [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string member = "")
        {
            logger.LogFatal(LogString(data, lineNumber, member));
        }
        internal static void LogInfo(object data)
        {
            logger.LogInfo(data);
        }
        internal static void LogMessage(object data)
        {
            logger.LogMessage(data);
        }
        internal static void LogWarning(object data)
        {
            logger.LogWarning(data);
        }

        private static string LogString(object data, [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string member = "")
        {
            return $"[{member}:{lineNumber}] {data}";
        }
    }
}
