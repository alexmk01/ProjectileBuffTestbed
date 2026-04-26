using Common.Tags;
using Common.Unity.Tags.Hierarchical;
using System;
using System.Collections.Generic;

namespace Common.Unity.Tags
{
    public static class TagReferenceExtensions
    {
        private static readonly List<Tag> TagsBuffer = new(8);
        private static readonly List<HierarchicalTagReference> TagReferencesBuffer = new(8);

        public static Tag GetValueOrDefault<T>(this T tagReference) where T : ITagReference
        {
            return tagReference is null ? Tag.Empty : tagReference.GetValue();
        }

        public static bool HasTag<T, U>(this U tagReferences, in Tag tag)
            where T : ITagReference
            where U : IEnumerable<T>
        {
            foreach (T tagReference in tagReferences)
            {
                if (tagReference.GetValue() == tag)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool HasAnyTag<T, U>(this U tagReferences)
            where T : ITagReference
            where U : IEnumerable<T>
        {
            foreach (T tagReference in tagReferences)
            {
                if (tagReference.GetValue() != Tag.Empty)
                {
                    return true;
                }
            }

            return false;
        }

        public static ReadOnlySpan<Tag> GetTags<T>(this ReadOnlySpan<T> tagReferences) where T : ITagReference
        {
            if (tagReferences.IsEmpty) return ReadOnlySpan<Tag>.Empty;
            TagsBuffer.Clear();

            for (int i = 0; i < tagReferences.Length; i++)
            {
                TagsBuffer.Add(tagReferences[i].GetValue());
            }

            return TagsBuffer.AsSpan();
        }

        public static ReadOnlySpan<HierarchicalTagReference> GetTagReferences(this ReadOnlySpan<Tag> tags)
        {
            if (tags.IsEmpty) return ReadOnlySpan<HierarchicalTagReference>.Empty;
            TagReferencesBuffer.Clear();

            for (int i = 0; i < tags.Length; i++)
            {
                TagReferencesBuffer.Add(new HierarchicalTagReference(tags[i]));
            }

            return TagReferencesBuffer.AsSpan();
        }
    }
}
