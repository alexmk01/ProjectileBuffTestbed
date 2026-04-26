using System;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Common.Unity
{
    [Serializable, TypeConverter(typeof(SerializableGUIDTypeConverter))]
    public struct SerializableGUID : IEquatable<SerializableGUID>
    {
        public static bool operator ==(SerializableGUID guid0, SerializableGUID guid1) => guid0.Equals(guid1);
        public static bool operator !=(SerializableGUID guid0, SerializableGUID guid1) => !(guid0 == guid1);

        public static explicit operator Hash128(SerializableGUID guid) => new((ulong)guid.u64_0, (ulong)guid.u64_1);

        public static readonly SerializableGUID Empty = new(Guid.Empty);

        public readonly bool IsValid => this != Empty;

        [SerializeField]
        private long u64_0;

        [SerializeField]
        private long u64_1;

        public SerializableGUID(long u64_0, long u64_1)
        {
            this.u64_0 = u64_0;
            this.u64_1 = u64_1;
        }

        public SerializableGUID(Guid guid)
        {
            Span<byte> guidBytes = stackalloc byte[16];
            guid.TryWriteBytes(guidBytes);
            u64_0 = MemoryMarshal.Read<long>(guidBytes[..8]);
            u64_1 = MemoryMarshal.Read<long>(guidBytes[8..16]);
        }

        public SerializableGUID(Hash128 hash)
        {
            Type hashType = typeof(Hash128);
            var fieldFlags = BindingFlags.Instance | BindingFlags.NonPublic;
            object boxedHash = hash;
            u64_0 = (long)hashType.GetField("u64_0", fieldFlags).GetValue(boxedHash);
            u64_1 = (long)hashType.GetField("u64_1", fieldFlags).GetValue(boxedHash);
        }

        public readonly void GetRawValue(out long u64_0, out long u64_1)
        {
            u64_0 = this.u64_0;
            u64_1 = this.u64_1;
        }

        public Guid AsGuid()
        {
            if (IsUnassigned()) return Guid.Empty;
            Span<byte> guidBytes = stackalloc byte[16];
            MemoryMarshal.Write(guidBytes[..8], ref u64_0);
            MemoryMarshal.Write(guidBytes[8..16], ref u64_1);
            return new Guid(guidBytes);
        }

        public readonly bool IsUnassigned() => u64_0 == 0 && u64_1 == 0;

        public readonly bool Equals(SerializableGUID other)
        {
            return u64_0 == other.u64_0 && u64_1 == other.u64_1;
        }

        public override readonly bool Equals(object obj)
        {
            return obj is SerializableGUID guid && Equals(guid);
        }

        public readonly override int GetHashCode() => HashCode.Combine(u64_0, u64_1);
        public override string ToString() => $"{AsGuid()} ({u64_0}, {u64_1})";
    }
}
