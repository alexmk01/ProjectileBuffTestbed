using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.Assertions;

namespace Common.Unity.Tags.Hierarchical.Editor
{
    public sealed class HierarchicalTagSelectionPopup : AdvancedDropdown
    {
        public sealed class TagItem : AdvancedDropdownItem
        {
            public AdvancedDropdownItem Parent { get; set; }

            public string tag;
            public Guid tagId;

            public TagItem(string fullTag, string localTag, Guid tagId) : base(localTag)
            {
                tag = fullTag;
                this.tagId = tagId;
            }

            public SerializableGUID GetSerializableTagGUID() => new(tagId);
            public HierarchicalTagReference GetHierarchicalTagReference() => new(GetSerializableTagGUID());
        }

        public const string EmptyTagLabel = "None";

        public SerializedProperty TagProperty { get; set; }
        public TagItem SelectedItem { get; private set; }

        public event Action<HierarchicalTagSelectionPopup> TagSelected;

        protected override AdvancedDropdownItem BuildRoot()
        {
            var root = new AdvancedDropdownItem("Tags");
            root.AddChild(new TagItem(string.Empty, EmptyTagLabel, Guid.Empty));
            AdvancedDropdownItem currentParentTagItem = root;
            AdvancedDropdownItem lastTagItem = root;
            int lastTagDepth = 0;

            foreach (KeyValuePair<Guid, string> tagData in HierarchicalTagStorage.GetCurrentTags())
            {
                Guid tagId = tagData.Key;
                string tag = tagData.Value;
                int tagDepth = HierarchicalTagUtility.SplitTag(tag, out Range[] localTags) - 1;
                Assert.IsTrue(tagDepth >= 0);
                string localTag = HierarchicalTagUtility.GetTag(tag, in localTags[tagDepth]);
                var newTagItem = new TagItem(tag, localTag, tagId);
                if (tagDepth == 0) currentParentTagItem = root;
                else if (tagDepth > lastTagDepth) currentParentTagItem = lastTagItem;
                else if (tagDepth < lastTagDepth) currentParentTagItem = ((TagItem)currentParentTagItem).Parent;
                newTagItem.Parent = currentParentTagItem;
                currentParentTagItem.AddChild(newTagItem);
                lastTagItem = newTagItem;
                lastTagDepth = tagDepth;
            }

            return root;
        }

        public HierarchicalTagSelectionPopup(AdvancedDropdownState state) : base(state)
        {
            minimumSize = new Vector2(200f, 300f);
        }

        protected override void ItemSelected(AdvancedDropdownItem item)
        {
            base.ItemSelected(item);
            SelectedItem = item as TagItem;
            TagSelected?.Invoke(this);
        }
    }
}
