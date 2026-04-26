using System.Diagnostics;
using UnityEngine;
using static Common.DebugUtils;
using static UnityEngine.Debug;

namespace Common.Unity
{
    public static class DebugUtils
    {
        private static void InitializeTraceListener()
        {
            Trace.Listeners.Clear();
            Trace.Listeners.Add(new UnityDebugTraceListener());
            Trace.AutoFlush = true;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void RuntimeInitialize()
        {
            EnableLogging();
            if (IsLoggingEnabled) InitializeTraceListener();
        }
        
    #if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
        private static void EditorInitialize()
        {
            EnableLogging();
            if (IsLoggingEnabled) InitializeTraceListener();
        }
    #endif
        
        [Conditional(EditorCondition), Conditional(DevBuildCondition), Conditional(ForcedLoggingCondition)]
        public static void Log(string message, Object context) => UnityEngine.Debug.Log(message, context);
        
        [Conditional(EditorCondition), Conditional(DevBuildCondition), Conditional(ForcedLoggingCondition)]
        public static void LogError(string message, Object context) => UnityEngine.Debug.LogError(message, context);
        
        [Conditional(EditorCondition), Conditional(DevBuildCondition), Conditional(ForcedLoggingCondition)]
        public static void LogWarning(string message, Object context) => UnityEngine.Debug.LogWarning(message, context);
        
        [Conditional(EditorCondition)]
        public static void DrawBounds(Bounds bounds, Color color, float duration = 0f, bool depthTest = true)
        {
            Vector3 center = bounds.center;

            float x = bounds.extents.x;
            float y = bounds.extents.y;
            float z = bounds.extents.z;

            Vector3 ruf = center + new Vector3(x, y, z);
            Vector3 rub = center + new Vector3(x, y, -z);
            Vector3 luf = center + new Vector3(-x, y, z);
            Vector3 lub = center + new Vector3(-x, y, -z);

            Vector3 rdf = center + new Vector3(x, -y, z);
            Vector3 rdb = center + new Vector3(x, -y, -z);
            Vector3 lfd = center + new Vector3(-x, -y, z);
            Vector3 lbd = center + new Vector3(-x, -y, -z);

            DrawLine(ruf, luf, color, duration, depthTest);
            DrawLine(ruf, rub, color, duration, depthTest);
            DrawLine(luf, lub, color, duration, depthTest);
            DrawLine(rub, lub, color, duration, depthTest);

            DrawLine(ruf, rdf, color, duration, depthTest);
            DrawLine(rub, rdb, color, duration, depthTest);
            DrawLine(luf, lfd, color, duration, depthTest);
            DrawLine(lub, lbd, color, duration, depthTest);

            DrawLine(rdf, lfd, color, duration, depthTest);
            DrawLine(rdf, rdb, color, duration, depthTest);
            DrawLine(lfd, lbd, color, duration, depthTest);
            DrawLine(lbd, rdb, color, duration, depthTest);
        }

        [Conditional(EditorCondition)]
        public static void DrawBounds(Vector3 min, Vector3 max, Color color, float duration = 0f, bool depthTest = true)
        {
            var bounds = new Bounds();
            bounds.SetMinMax(min, max);
            DrawBounds(bounds, color, duration, depthTest);
        }

        [Conditional(EditorCondition)]
        public static void DrawPoint(Vector3 position, Color color, float scale = 1f, float duration = 0f, bool depthTest = true)
        {
            color = (color == default) ? Color.white : color;
            DrawRay(position + (Vector3.up * (scale * 0.5f)), -Vector3.up * scale, color, duration, depthTest);
            DrawRay(position + (Vector3.right * (scale * 0.5f)), -Vector3.right * scale, color, duration, depthTest);
            DrawRay(position + (Vector3.forward * (scale * 0.5f)), -Vector3.forward * scale, color, duration, depthTest);
        }

        [Conditional(EditorCondition)]
        public static void DrawPoint(Vector3 position, float scale = 1f, float duration = 0f, bool depthTest = true)
        {
            DrawPoint(position, Color.white, scale, duration, depthTest);
        }

        [Conditional(EditorCondition)]
        public static void DrawCircle(Vector3 position, Vector3 up, Color color, float radius = 1f, float duration = 0f, bool depthTest = true)
        {
            up = up.normalized * radius;
            var forward = Vector3.Slerp(up, -up, 0.5f);
            Vector3 right = Vector3.Cross(up, forward).normalized * radius;

            var matrix = new Matrix4x4();
            matrix[0] = right.x;
            matrix[1] = right.y;
            matrix[2] = right.z;
            matrix[4] = up.x;
            matrix[5] = up.y;
            matrix[6] = up.z;
            matrix[8] = forward.x;
            matrix[9] = forward.y;
            matrix[10] = forward.z;

            Vector3 lastPoint = position + matrix.MultiplyPoint3x4(new Vector3(1f, 0f, 0f));
            Vector3 nextPoint = Vector3.zero;

            color = (color == default) ? Color.white : color;

            for (int i = 0; i < 91; i++)
            {
                nextPoint.x = Mathf.Cos(i * 4f * Mathf.Deg2Rad);
                nextPoint.z = Mathf.Sin(i * 4f * Mathf.Deg2Rad);
                nextPoint.y = 0f;
                nextPoint = position + matrix.MultiplyPoint3x4(nextPoint);
                DrawLine(lastPoint, nextPoint, color, duration, depthTest);
                lastPoint = nextPoint;
            }
        }

        [Conditional(EditorCondition)]
        public static void DrawCone(Vector3 position, Vector3 direction, Color color, float angle = 45f, float duration = 0f, bool depthTest = true)
        {
            float length = direction.magnitude;
            Vector3 forward = direction;
            var up = Vector3.Slerp(forward, -forward, 0.5f);
            Vector3 right = Vector3.Cross(forward, up).normalized * length;
            direction.Normalize();
            var slerpedVector = Vector3.Slerp(forward, up, angle / 90f);

            var farPlane = new Plane(-direction, position + forward);
            var distRay = new Ray(position, slerpedVector);
            farPlane.Raycast(distRay, out float dist);

            DrawRay(position, slerpedVector.normalized * dist, color);
            DrawRay(position, Vector3.Slerp(forward, -up, angle / 90f).normalized * dist, color, duration, depthTest);
            DrawRay(position, Vector3.Slerp(forward, right, angle / 90f).normalized * dist, color, duration, depthTest);
            DrawRay(position, Vector3.Slerp(forward, -right, angle / 90f).normalized * dist, color, duration, depthTest);
            DrawCircle(position + forward, direction, color, (forward - (slerpedVector.normalized * dist)).magnitude, duration, depthTest);
            DrawCircle(position + (forward * 0.5f), direction, color, ((forward * 0.5f) - (slerpedVector.normalized * (dist * 0.5f))).magnitude, duration, depthTest);
        }

        [Conditional(EditorCondition)]
        public static void DrawArrow(Vector3 position, Vector3 direction, Color color, float duration = 0, bool depthTest = true)
        {
            DrawRay(position, direction, color, duration, depthTest);
            DrawCone(position + direction, -direction * 0.333f, color, 15f, duration, depthTest);
        }

        [Conditional(EditorCondition)]
        public static void DrawBezier2(Vector3 p0, Vector3 p1, Vector3 p2, Color color, float duration = 0f, bool depthTest = true, int steps = 25)
        {
            Vector3 EvaluateBezier2(float t)
            {
                float invT = 1f - t;
                return invT * invT * p0 + 2f * invT * t * p1 + t * t * p2;
            }

            for (int i = 0; i < steps; i++)
            {
                DrawLine(EvaluateBezier2((float)i / steps), EvaluateBezier2((float)(i + 1) / steps), color, duration, depthTest);
            }
        }
    }
}