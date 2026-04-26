using System.Collections.Generic;
using Common.Tags;

namespace Common.Unity.Tags.Hierarchical
{
    public static class HierarchicalTagExtensions
    {
        public static SerializableGUID ToGuid(in this Tag tag) => tag.HasValue() ? new(HierarchicalTagUtility.GetTagId((string)tag)) : SerializableGUID.Empty;
        public static Tag ToTag(in this SerializableGUID id) => new HierarchicalTagReference(id).GetValue();

        public static void AddTags(this TagContainer tagContainer, IReadOnlyList<HierarchicalTagReference> tags)
        {
            for (int i = 0; i < tags.Count; i++)
            {
                tagContainer.AddTag(tags[i].GetValue());   
            }
        }

        public static void AddUniqueTags(this TagContainer tagContainer, IReadOnlyList<HierarchicalTagReference> tags)
        {
            for (int i = 0; i < tags.Count; i++)
            {
                tagContainer.AddUniqueTag(tags[i].GetValue());
            }
        }

        public static void RemoveTags(this TagContainer tagContainer, IReadOnlyList<HierarchicalTagReference> tags)
        {
            for (int i = 0; i < tags.Count; i++)
            {
                tagContainer.RemoveTag(tags[i].GetValue());
            }
        }
    }
}
