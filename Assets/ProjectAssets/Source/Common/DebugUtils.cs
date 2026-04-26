using System.Diagnostics;

namespace Common
{
    public static class DebugUtils
    {
        public const string EditorCondition = "UNITY_EDITOR";
        public const string DevBuildCondition = "DEVELOPMENT_BUILD";
        public const string ForcedLoggingCondition = "FORCED_LOGGING";
        
        public static bool IsLoggingEnabled { get; private set; }
        
        [Conditional(EditorCondition), Conditional(DevBuildCondition), Conditional(ForcedLoggingCondition)]
        public static void EnableLogging() => IsLoggingEnabled = true;

        [Conditional(EditorCondition), Conditional(DevBuildCondition), Conditional(ForcedLoggingCondition)]
        public static void Log(string message) => Trace.TraceInformation(message);
        
        [Conditional(EditorCondition), Conditional(DevBuildCondition), Conditional(ForcedLoggingCondition)]
        public static void LogError(string message) => Trace.TraceError(message);

        [Conditional(EditorCondition), Conditional(DevBuildCondition), Conditional(ForcedLoggingCondition)]
        public static void LogWarning(string message) => Trace.TraceWarning(message);
        
        [Conditional("UNITY_ASSERTIONS")]
        public static void Assert(bool condition, string message = null)
        {
            if (string.IsNullOrEmpty(message)) Debug.Assert(condition);
            else Debug.Assert(condition, message);
        }
    }
}