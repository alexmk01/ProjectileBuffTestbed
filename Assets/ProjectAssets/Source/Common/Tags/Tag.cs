using System;
using System.Collections.Generic;

namespace Common.Tags
{
    public readonly struct Tag : IEquatable<Tag>, IEquatable<string>, IEquatable<int>
    {
        public const StringComparison TagValueComparison = StringComparison.OrdinalIgnoreCase;
        
        public static readonly StringComparer TagValueComparer = StringComparer.OrdinalIgnoreCase;
        public static readonly Tag Empty = new();

        private static readonly Dictionary<string, int> RegisteredTags = new(256, TagValueComparer);
        private static readonly Dictionary<int, string> RegisteredTagValues = new(256);
        
        private static int lastTagId = 1;

        public static bool operator ==(Tag tag0, Tag tag1) => tag0.Equals(tag1);
        public static bool operator ==(Tag tag, int tagId) => tag.Equals(tagId);
        public static bool operator ==(Tag tag, string stringValue) => tag.Equals(stringValue);
        public static bool operator !=(Tag tag0, Tag tag1) => !(tag0 == tag1);
        public static bool operator !=(Tag tag, int tagId) => !(tag == tagId);
        public static bool operator !=(Tag tag, string stringValue) => !(tag == stringValue);
        public static explicit operator int(Tag tag) => tag.Id;
        public static explicit operator string(Tag tag) => tag.Value;
        public static explicit operator Tag(int value) => new(value);
        public static explicit operator Tag(string value) => new(value);

        public static int GetUniqueTagId() => checked(lastTagId++);

        private static int RegisterTag(string tag)
        {
            if (RegisteredTags.TryGetValue(tag, out int tagId))
            {
                return tagId;
            }
            
            tagId = GetUniqueTagId();
            DebugUtils.Assert(tagId != 0);
            RegisteredTags.Add(tag, tagId);
            RegisteredTagValues.Add(tagId, tag);
            return tagId;
        }

        internal readonly string Value;
        internal readonly int Id;

        public Tag(in Tag tag)
        {
            Value = tag.Value;
            Id = tag.Id;
        }

        public Tag(string value, int id)
        {
            Value = value;
            Id = id;
        }

        public Tag(int id)
        {
            Id = id;
            RegisteredTagValues.TryGetValue(id, out Value);
        }

        public Tag(string value)
        {
            Value = value;
            Id = RegisterTag(value);
        }

        public bool HasValue() => Id != Empty.Id;
        public bool Equals(Tag other) => other.Id == Id;
        public bool Equals(int tagId) => tagId == Id;
        public bool Equals(string other) => string.Equals(Value, other, TagValueComparison);
        public bool Contains(string tag) => Value != null && Value.Contains(tag, TagValueComparison);
        public bool Contains(in Tag tag) => Equals(tag) || Contains(tag.Value);
        public override bool Equals(object obj) => obj is Tag tag && Equals(tag);
        public override int GetHashCode() => Id;
        public override string ToString() => Value ?? Id.ToString();
    }
}
