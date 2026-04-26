using System;
using Common.Tags;
using Common.Unity.Tags.Hierarchical;
using UnityEngine;

namespace Common.Unity.Tags
{
    public sealed class TagContainerComponent : MonoBehaviour
    {
        public TagContainer Tags => tags;

        public event Action<TagContainer, Tag, bool> TagsModified;
        
        [SerializeField]
        private HierarchicalTagReference[] initialTags;
        
        private TagContainer tags;
        
        private void Awake()
        {
            void OnTagsModified(TagContainer tags, Tag tag, int tagIndex, bool isAdded)
            {
                TagsModified?.Invoke(tags, tag, isAdded);
            }
            
            tags = new TagContainer(Mathf.Max(16, initialTags.Length));
            tags.SetCallbacks(OnTagsModified, null);
            tags.AddTags(initialTags);
        }
        
        private void OnDestroy()
        {
            tags?.SetCallbacks(null, null);
        }
    }
}