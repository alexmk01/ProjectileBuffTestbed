using System;
using Common.Tags;

namespace Common.Unity.Tags.Hierarchical
{
    public static class HierarchicalTagUtility
    {
        public const char TagsDelimiter = '.';

        private static readonly Range[] TagSplittingBuffer = new Range[32];

        public static int GetTagDepth(string tagString)
        {
            if (string.IsNullOrEmpty(tagString)) return -1;
            int depth = 0;
            ReadOnlySpan<char> tagRawString = tagString.AsSpan();

            for (int i = tagRawString.Length - 2; i > 0; i--)
            {
                if (tagRawString[i] == TagsDelimiter && tagRawString[i - 1] != TagsDelimiter)
                {
                    depth++;
                }
            }

            return depth;
        }

        public static int SplitTag(string tagString, out Range[] tags)
        {
            if (string.IsNullOrEmpty(tagString))
            {
                tags = null;
                return 0;
            }

            int childTagStartIndex = 0;
            int childTagDepth = 0;
            ReadOnlySpan<char> tagSpan = tagString.AsSpan();
            tags = TagSplittingBuffer;

            for (int i = 1; i < tagSpan.Length - 1; i++)
            {
                if (tagSpan[i] == TagsDelimiter && tagSpan[i + 1] != TagsDelimiter)
                {
                    tags[childTagDepth] = childTagStartIndex..i;
                    childTagStartIndex = i + 1;
                    childTagDepth++;
                }
            }

            tags[childTagDepth] = childTagStartIndex..tagSpan.Length;
            return childTagDepth + 1;
        }

        public static int SplitTag(HierarchicalTagReference hierarchicalTag, out string tagString, out Range[] tags)
        {
            tagString = (string)hierarchicalTag.GetValue();
            return SplitTag(tagString, out tags);
        }

        public static ReadOnlySpan<char> GetTagNonAlloc(string tagString, in Range tagStringRange)
        {
            int start = tagStringRange.Start.Value;
            return tagString.AsSpan(start, tagStringRange.End.Value - start);
        }

        public static string GetTag(string tagString, in Range tagStringRange, bool convertToLowerCase = false)
        {
            ReadOnlySpan<char> stringSpan = GetTagNonAlloc(tagString, in tagStringRange);

            if (convertToLowerCase)
            {
                Span<char> lowerCaseString = stackalloc char[stringSpan.Length];
                int newCharCount = MemoryExtensions.ToLowerInvariant(stringSpan, lowerCaseString);
                return new string(lowerCaseString[..newCharCount]);
            }

            return new string(stringSpan);
        }

        public static string GetLocalTag(string tagString, int depth = -1, bool convertToLowerCase = false)
        {
            SplitTag(tagString, out Range[] tags);
            return GetTag(tagString, in tags[depth >= 0 ? depth : GetTagDepth(tagString)], convertToLowerCase);
        }

        public static string GetLocalTag(in Tag tag, int depth = -1, bool convertToLowerCase = false)
        {
            if (!tag.HasValue()) return string.Empty;
            return GetLocalTag(tag.ToString(), depth, convertToLowerCase);
        }

        public static Guid GetTagId(string tagValue, bool suppressErrorMessage = false)
        {
            if (HierarchicalTagStorage.TryGetTagId(tagValue, out Guid tagId))
            {
                return tagId;
            }
            
            if (!suppressErrorMessage) Common.DebugUtils.LogError($"{tagValue} tag is not registered.");
            return Guid.Empty;
        }

        public static HierarchicalTagReference GetTagReference(string tagValue, bool suppressErrorMessage = false)
        {
            if (HierarchicalTagStorage.TryGetTagId(tagValue, out Guid tagId))
            {
                return new HierarchicalTagReference(new SerializableGUID(tagId));
            }
            
            if (!suppressErrorMessage) Common.DebugUtils.LogError($"{tagValue} tag is not registered.");
            return new HierarchicalTagReference(SerializableGUID.Empty);
        }
        
        public static HierarchicalTagReference GetTagReference(string parentTags, string localTag)
        {
            return GetTagReference(GetFullTag(parentTags, localTag));
        }

        public static string GetFullTag(string parentTags, string localTag)
        {
            int fullTagLength = parentTags.Length + localTag.Length;
            ReadOnlySpan<char> parentTagSpan = parentTags.AsSpan();
            ReadOnlySpan<char> localTagSpan = localTag.AsSpan();
            bool addDelimiter = parentTagSpan[^1] != TagsDelimiter;
            if (addDelimiter) fullTagLength++;
            Span<char> fullTag = stackalloc char[fullTagLength];
            parentTagSpan.CopyTo(fullTag);
            int localTagStartIndex = parentTags.Length;

            if (addDelimiter)
            {
                fullTag[localTagStartIndex++] = TagsDelimiter;
            }

            for (int i = localTagStartIndex, j = 0; i < fullTagLength; i++, j++)
            {
                fullTag[i] = localTagSpan[j];
            }

            return new string(fullTag);
        }
    }
}
