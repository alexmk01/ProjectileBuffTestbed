using System;
using System.Collections.Generic;
using Common.Tags;
using UnityEngine;

namespace Common.Unity.Tags.Hierarchical
{
    [Serializable]
    public struct HierarchicalTagReference : IEquatable<HierarchicalTagReference>, ITagReference
    {
        private static readonly Dictionary<SerializableGUID, Tag> TagValues = new(256);
        
        public static explicit operator HierarchicalTagReference(Tag tag) => new(tag);
        public static explicit operator Tag(HierarchicalTagReference tagReference) => tagReference.GetValue();

        public static bool operator ==(HierarchicalTagReference tag0, HierarchicalTagReference tag1) => tag0.Equals(tag1);
        public static bool operator !=(HierarchicalTagReference tag0, HierarchicalTagReference tag1) => !(tag0 == tag1);

        public readonly SerializableGUID TagId => tagId;
        
        [SerializeField]
        private SerializableGUID tagId;

        public HierarchicalTagReference(SerializableGUID guid)
        {
            tagId = guid;
        }

        public HierarchicalTagReference(in Tag tag)
        {
            tagId = tag.ToGuid();
        }
        
        public HierarchicalTagReference(string tagValue)
        {
            tagId = !string.IsNullOrEmpty(tagValue) ? HierarchicalTagUtility.GetTagReference(tagValue).TagId : SerializableGUID.Empty;
        }

        public bool HasTag() => GetValue().HasValue();

        public Tag GetValue()
        {
            if (tagId.IsUnassigned())
            {
                return Tag.Empty;
            }

            if (!TagValues.TryGetValue(tagId, out Tag tag))
            {
                string tagValue = HierarchicalTagStorage.GetTagValue(tagId.AsGuid());
                tag = tagValue != null ? new(tagValue) : Tag.Empty;
                TagValues.Add(tagId, tag);
            }

            return tag;
        }
        
        public readonly bool Equals(HierarchicalTagReference other) => tagId == other.tagId;
        public readonly override bool Equals(object obj) => obj is HierarchicalTagReference tagReference && Equals(tagReference);
        public readonly override int GetHashCode() => tagId.GetHashCode();
        public override string ToString() => GetValue().ToString();
    }
}
