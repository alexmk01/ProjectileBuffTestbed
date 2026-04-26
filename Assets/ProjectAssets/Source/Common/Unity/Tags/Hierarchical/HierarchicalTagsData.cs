using System;
using System.Collections.Generic;
using System.IO;

namespace Common.Unity.Tags.Hierarchical
{
    [Serializable]
    public sealed class HierarchicalTagsData
    {
        [Serializable]
        public struct SerializableTag
        {
            public string tagId;
            public string tagString;

            internal readonly void WriteBinary(BinaryWriter writer)
            {
                writer.Write(tagId);
                writer.Write(tagString);
            }

            internal SerializableTag(BinaryReader reader)
            {
                tagId = reader.ReadString();
                tagString = reader.ReadString();
            }

            public SerializableTag(Guid tagId, string tagString)
            {
                this.tagId = tagId.ToString();
                this.tagString = tagString;
            }
        }

        public List<SerializableTag> tags;
    }
}
