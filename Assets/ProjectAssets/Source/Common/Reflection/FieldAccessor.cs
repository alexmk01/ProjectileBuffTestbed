using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Common.Reflection
{
    public readonly struct FieldAccessor
    {
        private static readonly BindingFlags FieldFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
        private static readonly List<(string fieldName, int fieldOffset)> FieldOffsetBuffer = new(64);
        private static readonly Dictionary<(Type type, string fieldName), int> FieldOffsets = new(64);
        private static readonly Dictionary<Type, (string fieldName, int fieldOffset)[]> ExposedFieldsOffsets = new(64);
        
        private static unsafe ref byte GetBaseRef(object obj)
        {
            IntPtr methodTablePtr = Unsafe.As<object, IntPtr>(ref obj);
            return ref Unsafe.AsRef<byte>((void*)methodTablePtr);
        }
        //https://meetemq.com/2023/09/10/nets-fields-and-their-offsets/
        public static unsafe int GetFieldOffset(FieldInfo fieldInfo)
        {
            DebugUtils.Assert(fieldInfo != null);
            int offset = 0;
            //check is mono/il2cpp
#pragma warning disable CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type
            if (sizeof(TypedReference) == IntPtr.Size * 3)
            {
                //pointer to MonoClassField
                var rhv = (IntPtr*)fieldInfo.FieldHandle.Value;
                //skip three pointers
                rhv += 3; 
                //load the value of a pointer (4 bytes, int32), then subtracting 16 bytes from it
                offset = *(int*)rhv - IntPtr.Size * 2; 
            }
            else
            {
                var rhv = (byte*)fieldInfo.FieldHandle.Value;
                //0x3FFFFF masks 22 bits
                //https://github.com/dotnet/runtime/blob/62d33ee48d57feba67b261b55db666bdc202b1c1/src/coreclr/vm/field.h#L36
                offset = *(int*)(rhv + IntPtr.Size + sizeof(int)) & 0x3FFFFF;
            }
#pragma warning restore CS8500

            if (fieldInfo.DeclaringType.IsClass)
            {
                offset += IntPtr.Size * 2;
            }
            
            return offset;
        }
        
        public static int GetFieldOffset(Type type, string fieldName)
        {
            var key = (type, fieldName);

            if (FieldOffsets.TryGetValue(key, out int offset))
            {
                return offset;
            }

            Type currentType = type;

            while (currentType != null && currentType != typeof(object))
            {
                if (currentType.GetField(fieldName, FieldFlags) is FieldInfo targetField)
                {
                    offset = GetFieldOffset(targetField);
                    FieldOffsets.Add(key, offset);
                    return offset;
                }

                currentType = currentType.BaseType;
            }
            
            return -1;
        }
        
        public static ref T GetFieldValueRef<T>(object targetObject, int fieldOffset)
        {
            ref byte baseRef = ref GetBaseRef(targetObject);
            return ref Unsafe.As<byte, T>(ref Unsafe.AddByteOffset(ref baseRef, (nuint)fieldOffset));
        }
        
        public static void GetExposedFields(object targetObject, List<FieldAccessor> fieldAccessors)
        {
            Type type = targetObject.GetType();
        
            if (!ExposedFieldsOffsets.TryGetValue(type, out var fieldsOffsets))
            {
                Type currentType = type;
                DebugUtils.Assert((FieldFlags & BindingFlags.DeclaredOnly) != 0);
                FieldOffsetBuffer.Clear();
                
                while (currentType != null && currentType != typeof(object))
                {
                    static bool IsTargetField(MemberInfo member, object obj)
                    {
                        var field = (FieldInfo)member;
                        return field.IsPublic || field.GetCustomAttribute<ExposeFieldAttribute>() != null;
                    }
                    
                    MemberInfo[] exposedFields = currentType.FindMembers(MemberTypes.Field, FieldFlags, IsTargetField, null);

                    for (int i = 0; i < exposedFields.Length; i++)
                    {
                        var field = (FieldInfo)exposedFields[i];
                        FieldOffsetBuffer.Add((field.Name, GetFieldOffset(field)));
                    }
                    
                    currentType = currentType.BaseType;
                }
                
                fieldsOffsets = FieldOffsetBuffer.ToArray();
                ExposedFieldsOffsets.Add(type, fieldsOffsets);
            }
        
            fieldAccessors.Clear();
        
            for (int i = 0; i < fieldsOffsets.Length; i++)
            {
                var (fieldName, fieldOffset) = fieldsOffsets[i];
                fieldAccessors.Add(new FieldAccessor(targetObject, fieldName, fieldOffset));
            }
        }

        public readonly string FieldName;

        private readonly object targetObject;
        private readonly int fieldOffset;

        private FieldAccessor(object targetObject, string fieldName, int fieldOffset)
        {
            DebugUtils.Assert(targetObject != null);
            this.targetObject = targetObject;
            this.fieldOffset = fieldOffset;
            FieldName = fieldName;
            DebugUtils.Assert(fieldOffset >= 0, $"{targetObject}.{FieldName}");
        }

        public FieldAccessor(string fieldName, object targetObject) :
            this(targetObject, fieldName, GetFieldOffset(targetObject.GetType(), fieldName))
        {
        }

        public T GetValue<T>() => GetFieldValueRef<T>(targetObject, fieldOffset);
        public void SetValue<T>(T value) => GetFieldValueRef<T>(targetObject, fieldOffset) = value;
    }
}