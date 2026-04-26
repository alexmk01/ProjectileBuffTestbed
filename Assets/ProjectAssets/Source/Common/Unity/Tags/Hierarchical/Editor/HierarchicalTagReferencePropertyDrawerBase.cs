using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Common.Unity.Tags.Hierarchical.Editor
{
    public abstract class HierarchicalTagReferencePropertyDrawerBase : PropertyDrawer
    {
        private const string TagIdRaw0PropertyName = "u64_0";
        private const string TagIdRaw1PropertyName = "u64_1";

        protected virtual string TagIdPropertyName => "tagId";
        
        private static void GetTagIdProperties(SerializedProperty tagProperty, out SerializedProperty idRawValue0, out SerializedProperty idRawValue1)
        {
            idRawValue0 = tagProperty.FindPropertyRelative(TagIdRaw0PropertyName);
            idRawValue1 = tagProperty.FindPropertyRelative(TagIdRaw1PropertyName);
        }

        private static Guid GetCurrentTagId(SerializedProperty tagProperty)
        {
            GetTagIdProperties(tagProperty, out SerializedProperty tagRawValue0Property, out SerializedProperty tagRawValue1Property);
            return new SerializableGUID(tagRawValue0Property.longValue, tagRawValue1Property.longValue).AsGuid();
        }

        private static void SetCurrentTagId(SerializedProperty tagProperty, Guid tagId)
        {
            GetTagIdProperties(tagProperty, out SerializedProperty tagRawValue0Property, out SerializedProperty tagRawValue1Property);
            new SerializableGUID(tagId).GetRawValue(out long u64_0, out long u64_1);
            tagRawValue0Property.longValue = u64_0;
            tagRawValue1Property.longValue = u64_1;
            tagProperty.serializedObject.ApplyModifiedProperties();
        }

        private readonly Dictionary<Guid, string> newTags = new(16);
        private HierarchicalTagsEditorWindow tagSelectionPopup;
        private SerializedProperty currentTagProperty;
        private Guid currentTagId;
        private string currentTagLabel;
        private bool isTagMissing;

        private SerializedProperty GetTagProperty(SerializedProperty parentProperty)
        {
            string propertyName = TagIdPropertyName;
            return string.IsNullOrEmpty(propertyName) ? parentProperty : parentProperty.FindPropertyRelative(propertyName);
        }

        protected virtual string GetTagReferenceDisplayName(string tagReference) => tagReference;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty tagProperty = GetTagProperty(property);
            currentTagProperty = tagProperty;

            if (Event.current.type == EventType.Repaint)
            {
                IReadOnlyDictionary<Guid, string> currentTags = HierarchicalTagStorage.GetCurrentTags();
                currentTagLabel = null;
                currentTagId = GetCurrentTagId(tagProperty);
                isTagMissing = false;

                if (!currentTags.TryGetValue(currentTagId, out currentTagLabel) && !newTags.TryGetValue(currentTagId, out currentTagLabel))
                {
                    isTagMissing = currentTagId != Guid.Empty;
                    currentTagLabel = isTagMissing ? "Missing" : HierarchicalTagSelectionPopup.EmptyTagLabel;
                }
                else
                {
                    currentTagLabel = GetTagReferenceDisplayName(currentTagLabel);
                }
            }

            position = EditorGUI.PrefixLabel(position, label);
            if (isTagMissing) GUI.contentColor = Color.red;
            bool showPopup = GUI.Button(position, currentTagLabel, EditorStyles.popup);
            GUI.contentColor = Color.white;

            if (showPopup)
            {
                if (tagSelectionPopup == null)
                {
                    tagSelectionPopup = ScriptableObject.CreateInstance<HierarchicalTagsEditorWindow>();
                    tagSelectionPopup.TagAdded += OnTagAdded;
                    tagSelectionPopup.TagRemoved += OnTagRemoved;
                    tagSelectionPopup.TagSelected += OnTagSelected;
                    tagSelectionPopup.TagsDataSaved += OnTagsDataSaved;
                }
                
                position = GUIUtility.GUIToScreenRect(position);
                var size = new Vector2(position.width, Mathf.Max(tagSelectionPopup.Height, 200f));
                tagSelectionPopup.ShowAsDropDown(position, size);
                tagSelectionPopup.SelectedTagId = currentTagId;
            }
        }

        private void OnTagAdded(HierarchicalTagsEditorWindow window, Guid tagId, string tag)
        {
            newTags.Add(tagId, tag);
        }

        private void OnTagRemoved(HierarchicalTagsEditorWindow window, Guid tagId, string tag)
        {
            if (currentTagId == tagId) currentTagId = Guid.Empty;
            newTags.Remove(tagId);
        }

        private void OnTagSelected(HierarchicalTagsEditorWindow window, Guid tagId, string tag)
        {
            SetCurrentTagId(currentTagProperty, tagId);
        }

        private void OnTagsDataSaved(HierarchicalTagsEditorWindow window)
        {
            newTags.Clear();
        }
    }
}
