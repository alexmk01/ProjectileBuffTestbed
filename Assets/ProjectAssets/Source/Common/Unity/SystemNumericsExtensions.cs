using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using NumericsVector2 = System.Numerics.Vector2;
using NumericsVector3 = System.Numerics.Vector3;
using NumericsQuaternion = System.Numerics.Quaternion;

namespace Common.Unity
{
    public static class SystemNumericsExtensions
    {
        public static Vector2 ToUnity(this NumericsVector2 vector) => UnsafeUtility.As<NumericsVector2, Vector2>(ref vector);
        public static Vector3 ToUnity(this NumericsVector3 vector) => UnsafeUtility.As<NumericsVector3, Vector3>(ref vector);
        public static Quaternion ToUnity(this NumericsQuaternion quaternion) => UnsafeUtility.As<NumericsQuaternion, Quaternion>(ref quaternion);
        
        public static NumericsVector2 ToNumerics(this Vector2 vector) => UnsafeUtility.As<Vector2, NumericsVector2>(ref vector);
        public static NumericsVector3 ToNumerics(this Vector3 vector) => UnsafeUtility.As<Vector3, NumericsVector3>(ref vector);
        public static NumericsQuaternion ToNumerics(this Quaternion quaternion) => UnsafeUtility.As<Quaternion, NumericsQuaternion>(ref quaternion);

        public static Vector2 ToVector2(this NumericsVector3 vector)
        {
            var vector2 = new NumericsVector2(vector.X, vector.Y);
            return UnsafeUtility.As<NumericsVector2, Vector2>(ref vector2);
        }

        public static Vector3 ToVector3(this NumericsVector2 vector)
        {
            var vector3 = new NumericsVector3(vector.X, vector.Y, 0f);
            return UnsafeUtility.As<NumericsVector3, Vector3>(ref vector3);
        }
        
        public static NumericsVector2 ToNumericsVector2(this Vector3 vector)
        {
            var vector2 = (Vector2)vector;
            return UnsafeUtility.As<Vector2, NumericsVector2>(ref vector2);
        }
        
        public static NumericsVector3 ToNumericsVector3(this Vector2 vector)
        {
            var vector3 = (Vector3)vector;
            return UnsafeUtility.As<Vector3, NumericsVector3>(ref vector3);
        }
    }
}