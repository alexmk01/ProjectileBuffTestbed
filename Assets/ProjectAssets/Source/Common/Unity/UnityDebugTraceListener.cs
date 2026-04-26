using System.Diagnostics;

namespace Common.Unity
{
    public sealed class UnityDebugTraceListener : TraceListener
    {
        public override void Write(string message)
        {
            if (string.IsNullOrEmpty(message)) return;
            UnityEngine.Debug.Log(message); 
        }

        public override void WriteLine(string message)
        {
            if (string.IsNullOrEmpty(message)) return;
            UnityEngine.Debug.Log(message);
        }

        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
        {
            string fullMessage = string.IsNullOrEmpty(source) ? message : $"[{source}] {message}";

            switch (eventType)
            {
                case TraceEventType.Error:
                case TraceEventType.Critical:
                    UnityEngine.Debug.LogError(fullMessage);
                    break;

                case TraceEventType.Warning:
                    UnityEngine.Debug.LogWarning(fullMessage);
                    break;

                case TraceEventType.Information:
                case TraceEventType.Verbose:
                case TraceEventType.Start:
                case TraceEventType.Stop:
                case TraceEventType.Suspend:
                case TraceEventType.Resume:
                case TraceEventType.Transfer:
                    UnityEngine.Debug.Log(fullMessage);
                    break;
                
                default:
                    UnityEngine.Debug.Log(fullMessage);
                    break;
            }
        }

        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, params object[] args)
        {
            string message = (args == null || args.Length == 0) ? format : string.Format(format, args);
            TraceEvent(eventCache, source, eventType, id, message);
        }

        public override void Fail(string message) => UnityEngine.Debug.LogError($"[FAIL] {message}");
        public override void Fail(string message, string detailMessage) => UnityEngine.Debug.LogError($"[FAIL] {message}\n{detailMessage}");
        
        public override void WriteLine(string message, string category)
        {
            if (string.IsNullOrEmpty(category)) WriteLine(message);
            else WriteLine($"[{category}] {message}");
        }
    }
}