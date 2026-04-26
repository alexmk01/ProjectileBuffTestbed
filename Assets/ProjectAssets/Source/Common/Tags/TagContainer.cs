using System;
using System.Collections;
using System.Collections.Generic;

namespace Common.Tags
{
    public sealed class TagContainer : IEnumerable<Tag>
    {
        private sealed class Enumerator : IEnumerator<Tag>, IEnumerator
        {
            public Tag Current => current;
            object IEnumerator.Current => current;

            private readonly TagContainer container;
            private int count;
            private int index;
            private Tag current;

            public Enumerator(TagContainer container)
            {
                this.container = container;
                Reset();
            }

            public bool MoveNext()
            {
                DebugUtils.Assert(container.tagsCount == count);
                Tag[] tags = container.tags;

                if (index < count)
                {
                    current = tags[index];
                    index++;
                    return true;
                }

                return false;
            }

            public void Reset()
            {
                count = container.tagsCount;
                index = 0;
                current = new Tag();
            }

            public void Dispose() { }
        }

        public int TagsCount => tagsCount;
        public ref readonly Tag this[int tagIndex] => ref tags[tagIndex];

        private Action<TagContainer, Tag, int, bool> onModifiedCallback;
        private Action<TagContainer, Tag, int, int> onTagIndexChangedCallback;

        private Tag[] tags;
        private int tagsCount;

        private void OnContainerModified(in Tag tag, int tagIndex, bool isAdded)
        {
            onModifiedCallback?.Invoke(this, tag, tagIndex, isAdded);
        }

        private void OnTagIndexChanged(in Tag tag, int lastIndex, int newIndex)
        {
            onTagIndexChangedCallback?.Invoke(this, tag, lastIndex, newIndex);
        }

        public TagContainer(int initialCapacity = 16)
        {
            tags = new Tag[initialCapacity];
            tagsCount = 0;
        }

        public void SetCallbacks
        (
            Action<TagContainer, Tag, int, bool> onModifiedCallback,
            Action<TagContainer, Tag, int, int> onTagIndexChangedCallback
        )
        {
            this.onModifiedCallback = onModifiedCallback;
            this.onTagIndexChangedCallback = onTagIndexChangedCallback;
        }

        public ReadOnlySpan<Tag> AsSpan() => tags[..tagsCount];

        public int GetTagIndex(in Tag tag)
        {
            int tagHash = (int)tag;

            for (int i = tagsCount - 1; i >= 0; i--)
            {
                if (tags[i].Equals(tagHash))
                {
                    return i;
                }
            }

            return -1;
        }

        public bool HasTag(in Tag tag) => GetTagIndex(in tag) != -1;

        public bool HasRelatedTag(in Tag tag)
        {
            for (int i = tagsCount - 1; i >= 0; i--)
            {
                if (tags[i].Contains(in tag))
                {
                    return true;
                }
            }

            return false;
        }
        
        public void AddTag(in Tag tag)
        {
            DebugUtils.Assert(tag != Tag.Empty);

            if (tagsCount == tags.Length)
            {
                Array.Resize(ref tags, tagsCount * 2);
            }

            int tagIndex = tagsCount++;
            tags[tagIndex] = tag;
            OnContainerModified(in tag, tagIndex, true);
        }

        public void AddTags(IReadOnlyList<Tag> tags)
        {
            for (int i = 0; i < tags.Count; i++)
            {
                AddTag(tags[i]);
            }
        }

        public bool AddUniqueTag(in Tag tag)
        {
            if (GetTagIndex(in tag) == -1)
            {
                AddTag(in tag);
                return true;
            }

            return false;
        }

        public void AddUniqueTags(IReadOnlyList<Tag> tags)
        {
            for (int i = 0; i < tags.Count; i++)
            {
                AddUniqueTag(tags[i]);
            }
        }

        public bool RemoveTag(int tagIndex)
        {
            if (tagIndex != -1)
            {
                int lastTagIndex = tagsCount - 1;
                Tag tagToRemove = tags[tagIndex];

                if (tagIndex != lastTagIndex)
                {
                    Tag lastTag = tags[lastTagIndex];
                    tags[tagIndex] = lastTag;
                    OnTagIndexChanged(lastTag, lastTagIndex, tagIndex);
                }

                tags[lastTagIndex] = new Tag();
                tagsCount = lastTagIndex;
                OnContainerModified(in tagToRemove, tagIndex, false);
                return true;
            }

            return false;
        }

        public bool RemoveTag(in Tag tag) => RemoveTag(GetTagIndex(in tag));

        public bool RemoveRelatedTags(in Tag tag, bool equalOnly = false)
        {
            int i = 0;
            bool hadRelatedTags = false;

            while (i < tagsCount)
            {
                Tag currentTag = tags[i];

                if (equalOnly ? currentTag == tag : currentTag.Contains(in tag))
                {
                    RemoveTag(i);
                    hadRelatedTags = true;
                    continue;
                }

                i++;
            }

            return hadRelatedTags;
        }

        public void RemoveAllTags()
        {
            int lastTagIndex = tagsCount - 1;
            tagsCount = 0;

            for (int i = lastTagIndex; i >= 0; i--)
            {
                OnContainerModified(in tags[i], i, false);
                tags[i] = new Tag();
            }
        }

        IEnumerator<Tag> IEnumerable<Tag>.GetEnumerator() => new Enumerator(this);
        IEnumerator IEnumerable.GetEnumerator() => new Enumerator(this);
    }
}
