using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Common.Unity.Tags.Hierarchical.Editor
{
    public sealed class HierarchicalTagsEditorWindow : UnityEditor.EditorWindow
    {
        private sealed class TagTreeViewNode : TreeViewItem<int>
        {
            public const int RootNodeId = 0;

            private static int nodeId;

            public static int GetNodeId()
            {
                if (nodeId == RootNodeId) nodeId = RootNodeId + 1;
                unchecked { return nodeId++; }
            }

            public static bool SameTags(string tag0, string tag1)
            {
                return string.Equals(tag0, tag1, StringComparison.OrdinalIgnoreCase);
            }

            public static bool IsValidTag(string tag)
            {
                //TODO: use regex pattern, exclude forbidden chars etc
                return !string.IsNullOrEmpty(tag) && !string.IsNullOrWhiteSpace(tag) && HierarchicalTagUtility.GetTagDepth(tag) == 0;
            }

            public string FullTag
            {
                get => fullTag;
                set
                {
                    if (SameTags(fullTag, value)) return;
                    fullTag = value;
                    depth = HierarchicalTagUtility.GetTagDepth(value);
                    string newLocalTag = GetLocalTag();
                    displayName = newLocalTag;
                    UpdateChildNodesLocalTag(newLocalTag);
                }
            }

            public readonly Guid TagId;
            private string fullTag;
            
            private Range GetLocalTagStringRange(int depth)
            {
                HierarchicalTagUtility.SplitTag(fullTag, out Range[] tags);
                return tags[depth];
            }
            
            private void UpdateChildNodesLocalTag(string newLocalTag)
            {
                List<TreeViewItem<int>> childNodes = children;

                if (childNodes != null)
                {
                    for (int i = 0; i < childNodes.Count; i++)
                    {
                        (childNodes[i] as TagTreeViewNode)?.UpdateLocalTag(newLocalTag, depth);
                    }
                }
            }

            public TagTreeViewNode(Guid tagId, string tag)
            {
                id = GetNodeId();
                TagId = tagId;
                FullTag = tag;
            }

            public string GenerateChildNodeFullTag(string childNodeLocalTag)
            {
                return fullTag + HierarchicalTagUtility.TagsDelimiter + childNodeLocalTag;
            }

            public string GetLocalTag()
            {
                return HierarchicalTagUtility.GetLocalTag(fullTag);
            }

            public void UpdateLocalTag(string newLocalTag, int tagDepth = -1)
            {
                Range localTagRange = GetLocalTagStringRange(tagDepth < 0 ? depth : tagDepth);
                int currentLocalTagStart = localTagRange.Start.Value;
                int newLocalTagEnd = currentLocalTagStart + newLocalTag.Length;
                int newTagSizeDifference = localTagRange.End.Value - currentLocalTagStart - newLocalTag.Length;
                ReadOnlySpan<char> currentFullTag = fullTag.AsSpan();
                Span<char> newFullTag = stackalloc char[fullTag.Length - newTagSizeDifference];

                for (int i = 0; i < newFullTag.Length; i++)
                {
                    if (i < currentLocalTagStart)
                    {
                        newFullTag[i] = currentFullTag[i];
                    }
                    else if (i < newLocalTagEnd)
                    {
                        newFullTag[i] = newLocalTag[i - currentLocalTagStart];
                    }
                    else
                    {
                        newFullTag[i] = currentFullTag[i + newTagSizeDifference];
                    }
                }

                FullTag = new(newFullTag);
            }

            public HierarchicalTagsData.SerializableTag GetSerializableData()
            {
                return new HierarchicalTagsData.SerializableTag(TagId, fullTag);
            }
        }

        private sealed class TagTreeView : TreeView<int>
        {
            private sealed class NodeOperationArgs
            {
                public TreeViewItem<int> currentNode;
            }

            public const string DefaultLocalTag = "Tag";

            private static readonly List<HierarchicalTagsData.SerializableTag> SerializationDataBuffer = new(256);
            private static readonly GUIContent AddTagLabel = new("Add Tag");
            private static readonly GUIContent InsertTagLabel = new("Insert Tag");
            private static readonly GUIContent UnparentTagsLabel = new("Unparent Tags");
            private static readonly GUIContent RemoveTagLabel = new("Remove Tag");
            private static readonly GUIContent AddTagButtonLabel = new("", "Add Tag");
            private static readonly GUIContent InsertTagButtonLabel = new("", "Insert Tag");
            private static readonly GUIContent RemoveTagButtonLabel = new("", "Remove Tag");

            private static GUIStyle addTagButtonStyle;
            private static GUIStyle insertTagButtonStyle;
            private static GUIStyle removeTagButtonStyle;

            private static Texture2D CloneTexture(Texture2D sourceTexture, Color tint)
            {
                Texture2D textureClone = new(sourceTexture.width, sourceTexture.height, sourceTexture.format, false);
                textureClone.LoadRawTextureData(sourceTexture.GetRawTextureData());

                if (tint.a > 0f)
                {
                    Color32[] pixels = textureClone.GetPixels32();

                    for (int i = 0; i < pixels.Length; i++)
                    {
                        pixels[i] *= tint;
                    }

                    textureClone.SetPixels32(pixels);
                }

                textureClone.Apply();
                return textureClone;
            }

            private static GUIStyle CreateButtonStyle(GUIStyle sourceStyle, Texture2D icon, Color idleColor, Color hoverColor, Color activeColor)
            {
                GUIStyle style = new(sourceStyle)
                {
                    fixedHeight = 0f,
                    fixedWidth = 0f
                };

                Texture2D idleIcon = CloneTexture(icon, idleColor);
                Texture2D hoverIcon = CloneTexture(icon, hoverColor);
                Texture2D activeIcon = CloneTexture(icon, activeColor);
                style.normal.background = idleIcon;
                style.onNormal.background = idleIcon;
                style.hover.background = hoverIcon;
                style.onHover.background = hoverIcon;
                style.active.background = activeIcon;
                style.onActive.background = activeIcon;
                return style;
            }

            public static string CreateNewTag(TreeViewItem<int> parentNode, out Guid tagId)
            {
                tagId = HierarchicalTagStorage.GenerateTagId();
                return (parentNode as TagTreeViewNode)?.GenerateChildNodeFullTag(DefaultLocalTag) ?? DefaultLocalTag;
            }

            public static TagTreeViewNode CreateNewNode(TreeViewItem<int> parentNode, bool registerInParentNode = true)
            {
                string newTag = CreateNewTag(parentNode, out Guid tagID);
                var newNode = new TagTreeViewNode(tagID, newTag);
                if (registerInParentNode) parentNode.AddChild(newNode);
                return newNode;
            }

            private readonly HierarchicalTagsEditorWindow window;
            private TreeViewItem<int> currentRoot;
            private TreeViewItem<int> nullTagItem;
            private bool isDirty;

            private TagTreeViewNode FindTagItem(Guid tagId)
            {
                static void Find(TreeViewItem<int> currentItem, ref TagTreeViewNode foundItem, ref Guid tagId)
                {
                    if (foundItem != null)
                    {
                        return;
                    }

                    if (currentItem is TagTreeViewNode tagItem && tagItem.TagId == tagId)
                    {
                        foundItem = tagItem;
                        return;
                    }

                    List<TreeViewItem<int>> childItems = currentItem.children;
                    if (childItems == null || childItems.Count == 0) return;

                    for (int i = 0; i < childItems.Count; i++)
                    {
                        Find(childItems[i], ref foundItem, ref tagId);
                    }
                }

                TagTreeViewNode foundItem = null;
                Find(rootItem, ref foundItem, ref tagId);
                return foundItem;
            }

            public void SelectTagItem(TreeViewItem<int> item)
            {
                int[] selection = new int[1];
                if (item != null) selection[0] = item.id;
                else if (nullTagItem != null) selection[0] = nullTagItem.id;
                else selection = Array.Empty<int>();
                SetSelection(selection, TreeViewSelectionOptions.RevealAndFrame | TreeViewSelectionOptions.FireSelectionChanged);
            }

            public void SelectTagItem(Guid tagId) => SelectTagItem(FindTagItem(tagId));

            private void SetDirty() => isDirty = true;

            private void ShowNodeMenu(int nodeId)
            {
                TreeViewItem<int> node = nodeId == TagTreeViewNode.RootNodeId ? rootItem : FindItem(nodeId, rootItem);
                var args = new NodeOperationArgs { currentNode = node };
                var menu = new GenericMenu();
                menu.AddItem(AddTagLabel, false, OnAddTagOperationSelected, args);

                if (node != null)
                {
                    if (node != rootItem)
                    {
                        if (node.parent != null)
                        {
                            menu.AddItem(InsertTagLabel, false, OnInsertTagOperationSelected, args);
                        }

                        if (node.hasChildren)
                        {
                            menu.AddItem(UnparentTagsLabel, false, OnUnparentTagsOperationSelected, args);
                        }

                        menu.AddItem(RemoveTagLabel, false, OnRemoveTagOperationSelected, args);
                    }
                }

                menu.ShowAsContext();
            }

            protected override TreeViewItem<int> BuildRoot()
            {
                if (currentRoot != null) return currentRoot;
                var root = new TreeViewItem<int>(TagTreeViewNode.RootNodeId, -1, "Root");
                nullTagItem = new TreeViewItem<int>(TagTreeViewNode.GetNodeId(), 0, "None");
                IReadOnlyDictionary<Guid, string> currentTagsData = HierarchicalTagStorage.GetCurrentTags();
                var nodes = new List<TreeViewItem<int>>(currentTagsData.Count) { nullTagItem };

                foreach (KeyValuePair<Guid, string> tagData in currentTagsData)
                {
                    nodes.Add(new TagTreeViewNode(tagData.Key, tagData.Value));
                }

                SetupParentsAndChildrenFromDepths(root, nodes);
                currentRoot = root;
                return root;
            }

            protected override bool CanRename(TreeViewItem<int> item) => item.depth >= 0 && item is TagTreeViewNode;
            protected override bool CanMultiSelect(TreeViewItem<int> item) => false;

            protected override bool DoesItemMatchSearch(TreeViewItem<int> item, string search)
            {
                if (item.id == TagTreeViewNode.RootNodeId) return false;
                return item is TagTreeViewNode treeNode && treeNode.FullTag.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0;
            }

            protected override void RenameEnded(RenameEndedArgs args)
            {
                string newLocalTag = args.newName;

                if (args.acceptedRename && TagTreeViewNode.IsValidTag(newLocalTag) && !TagTreeViewNode.SameTags(newLocalTag, args.originalName))
                {
                    (FindItem(args.itemID, rootItem) as TagTreeViewNode)?.UpdateLocalTag(newLocalTag);
                }

                base.RenameEnded(args);
                SetDirty();
            }

            protected override void BeforeRowsGUI()
            {
                var buttonStyle = new GUIStyle("OL Plus");
                addTagButtonStyle ??= CreateButtonStyle(buttonStyle, EditorGUIUtility.FindTexture("Toolbar Plus"), Color.clear, Color.cyan, Color.gray);
                insertTagButtonStyle ??= CreateButtonStyle(buttonStyle, EditorGUIUtility.FindTexture("Toolbar Plus More"), Color.clear, Color.cyan, Color.gray);
                removeTagButtonStyle ??= CreateButtonStyle(buttonStyle, EditorGUIUtility.FindTexture("Toolbar Minus"), Color.clear, Color.cyan, Color.gray);
                base.BeforeRowsGUI();
            }

            protected override void RowGUI(RowGUIArgs args)
            {
                base.RowGUI(args);
                const float margin = 2f;
                Rect rowRect = args.rowRect;
                //float buttonSize = rowRect.height - margin;
                float buttonSize = rowRect.height;

                if (!args.isRenaming && args.selected && args.item is TagTreeViewNode)
                {
                    Rect buttonRect = new(rowRect.xMax - buttonSize - margin, rowRect.y, buttonSize, buttonSize);

                    if (GUI.Button(buttonRect, RemoveTagButtonLabel, removeTagButtonStyle))
                    {
                        var operationArgs = new NodeOperationArgs { currentNode = args.item };
                        OnRemoveTagOperationSelected(operationArgs);
                        Event.current.Use();
                    }

                    buttonRect.x -= buttonSize + margin;

                    if (GUI.Button(buttonRect, AddTagButtonLabel, addTagButtonStyle))
                    {
                        var operationArgs = new NodeOperationArgs { currentNode = args.item };
                        OnAddTagOperationSelected(operationArgs);
                        Event.current.Use();
                    }

                    buttonRect.x -= buttonSize + margin;

                    if (GUI.Button(buttonRect, InsertTagButtonLabel, insertTagButtonStyle))
                    {
                        var operationArgs = new NodeOperationArgs { currentNode = args.item };
                        OnInsertTagOperationSelected(operationArgs);
                        Event.current.Use();
                    }
                }

                if (args.row == GetRows().Count - 1)
                {
                    Rect buttonRect = args.rowRect;
                    buttonRect.width = buttonSize;
                    buttonRect.x = args.rowRect.center.x - buttonSize * 0.5f;
                    buttonRect.y += buttonRect.height;

                    if (GUI.Button(buttonRect, InsertTagButtonLabel, insertTagButtonStyle))
                    {
                        var operationArgs = new NodeOperationArgs { currentNode = rootItem };
                        OnAddTagOperationSelected(operationArgs);
                        Event.current.Use();
                    }
                }
            }

            protected override void SelectionChanged(IList<int> selectedIds)
            {
                TagTreeViewNode newTagNode = null;

                if (selectedIds != null && selectedIds.Count != 0)
                {
                    newTagNode = FindItem(selectedIds[0], rootItem) as TagTreeViewNode;
                }

                window.OnTagNodeSelected(newTagNode);
            }

            protected override void ContextClicked()
            {
                ShowNodeMenu(rootItem.id);
                base.ContextClicked();
                Event.current.Use();
            }

            protected override void ContextClickedItem(int id)
            {
                ShowNodeMenu(id);
                base.ContextClickedItem(id);
                Event.current.Use();
            }

            public TagTreeView(HierarchicalTagsEditorWindow window, TreeViewState<int> state) : base(state)
            {
                this.window = window;
                showBorder = true;
                showAlternatingRowBackgrounds = true;
                enableItemHovering = true;
                Reload();
            }
            
            public List<HierarchicalTagsData.SerializableTag> GetSerializableData()
            {
                static void CollectSerializableData(TreeViewItem<int> node, List<HierarchicalTagsData.SerializableTag> data)
                {
                    if (node.depth >= 0 && node is TagTreeViewNode tagNode)
                    {
                        data.Add(tagNode.GetSerializableData());
                    }
                    
                    List<TreeViewItem<int>> childNodes = node.children;

                    if (childNodes != null)
                    {
                        for (int i = 0; i < childNodes.Count; i++)
                        {
                            CollectSerializableData(childNodes[i], data);
                        }
                    }
                }

                SerializationDataBuffer.Clear();
                CollectSerializableData(rootItem, SerializationDataBuffer);
                return SerializationDataBuffer;
            }

            public void Save()
            {
                if (!isDirty) return;
                HierarchicalTagStorage.SaveTagsData(GetSerializableData(), true);
                window.OnTagsDataSaved();
                isDirty = false;
            }

            private void OnAddTagOperationSelected(object args)
            {
                var operationArgs = (NodeOperationArgs)args;
                OnNewNodeAdded(CreateNewNode(operationArgs.currentNode));
            }
            
            private void OnInsertTagOperationSelected(object args)
            {
                var operationArgs = (NodeOperationArgs)args;
                TreeViewItem<int> parentNode = operationArgs.currentNode.parent;
                List<TreeViewItem<int>> neighbourNodes = parentNode.children;
                int currentNodeIndex = neighbourNodes.IndexOf(operationArgs.currentNode);
                TagTreeViewNode newNode = CreateNewNode(parentNode, false);
                neighbourNodes.Insert(currentNodeIndex + 1, newNode);
                newNode.parent = parentNode;
                OnNewNodeAdded(newNode);
            }

            private void OnUnparentTagsOperationSelected(object args)
            {
                var operationArgs = (NodeOperationArgs)args;
                List<TreeViewItem<int>> childNodes = operationArgs.currentNode.children;
                var parentNode = (TagTreeViewNode)operationArgs.currentNode.parent;
                string parentNodeTag = parentNode.FullTag;

                for (int i = 0; i < childNodes.Count; i++)
                {
                    var childNode = (TagTreeViewNode)childNodes[i];
                    childNode.FullTag = HierarchicalTagUtility.GetFullTag(parentNodeTag, childNode.GetLocalTag());
                    parentNode.AddChild(childNode);
                }

                childNodes.Clear();
                Reload();
                SetDirty();
            }

            private void OnRemoveTagOperationSelected(object args)
            {
                var operationArgs = (NodeOperationArgs)args;
                TreeViewItem<int> nodeToRemove = operationArgs.currentNode;

                if (nodeToRemove is TagTreeViewNode tagNode)
                {
                    window.OnTagNodeRemoved(tagNode);
                }

                List<TreeViewItem<int>> childNodes = nodeToRemove.children;

                if (childNodes != null && childNodes.Count != 0)
                {
                    foreach (TreeViewItem<int> childNode in childNodes)
                    {
                        childNode.parent = null;
                    }

                    childNodes.Clear();
                }

                nodeToRemove.parent?.children?.Remove(nodeToRemove);
                nodeToRemove.parent = null;
                Reload();
                SetDirty();
            }

            private void OnNewNodeAdded(TreeViewItem<int> newNode)
            {
                Reload();
                FrameItem(newNode.id);
                BeginRename(newNode);
                SetDirty();

                if (newNode is TagTreeViewNode tagNode)
                {
                    window.OnTagNodeAdded(tagNode);
                }
            }
        }

        private sealed class AssetsModificationListener : AssetModificationProcessor
        {
            public static event Action BeforeProjectSaved;

            private static string[] OnWillSaveAssets(string[] paths)
            {
                BeforeProjectSaved?.Invoke();
                return paths;
            }
        }

        [MenuItem("Window/Tags")]
        private static void ShowWindow()
        {
            var window = GetWindow<HierarchicalTagsEditorWindow>();
            Rect mainWindowPosition = EditorGUIUtility.GetMainWindowPosition();
            Rect position = window.position;
            position.width = 400f;
            position.height = 700f;
            position.x = mainWindowPosition.x + (mainWindowPosition.width - position.width) * 0.5f;
            position.y = mainWindowPosition.y + (mainWindowPosition.height - position.height) * 0.5f;
            window.position = position;
            window.titleContent = new GUIContent("Tags");
            window.wantsMouseMove = true;
            window.Show();
        }

        public float Height => searchBarHeight + yMargin + tagTreeView.totalHeight;

        public Guid SelectedTagId
        {
            get => selectedTagId;
            set
            {
                if (selectedTagId == value) return;
                selectedTagId = value;
                tagTreeView.SelectTagItem(value);
            }
        }

        public event Action<HierarchicalTagsEditorWindow, Guid, string> TagAdded;
        public event Action<HierarchicalTagsEditorWindow, Guid, string> TagRemoved;
        public event Action<HierarchicalTagsEditorWindow, Guid, string> TagSelected;
        public event Action<HierarchicalTagsEditorWindow> TagsDataSaved;

        [SerializeField]
        private TreeViewState<int> tagTreeState;

        private TagTreeView tagTreeView;
        private SearchField searchField;
        private Guid selectedTagId;

        private void OnEnable()
        {
            tagTreeState ??= new TreeViewState<int>();

            if (tagTreeView == null)
            {
                tagTreeView = new TagTreeView(this, tagTreeState);
                searchField = new SearchField();
            }

            searchField.downOrUpArrowKeyPressed += tagTreeView.SetFocusAndEnsureSelectedItem;
            AssetsModificationListener.BeforeProjectSaved += OnSavingProject;
        }
        
        private void OnTagNodeAdded(TagTreeViewNode node)
        {
            TagAdded?.Invoke(this, node.TagId, node.FullTag);
        }

        private void OnTagNodeRemoved(TagTreeViewNode node)
        {
            TagRemoved?.Invoke(this, node.TagId, node.FullTag);
        }

        private void OnTagNodeSelected(TagTreeViewNode node)
        {
            var newTagId = node?.TagId ?? Guid.Empty;
            if (newTagId == selectedTagId) return;
            selectedTagId = newTagId;
            TagSelected?.Invoke(this, newTagId, node?.FullTag ?? string.Empty);
        }

        private void OnTagsDataSaved()
        {
            TagsDataSaved?.Invoke(this);
        }

        private void OnSavingProject()
        {
            tagTreeView.Save();
        }

        const float xMargin = 10f;
        const float yMargin = 10f;
        const float searchBarHeight = 20f;

        private void OnGUI()
        {
            string searchString = searchField.OnGUI(new Rect(xMargin, yMargin, position.width - xMargin * 2f, searchBarHeight), tagTreeView.searchString);
            tagTreeView.searchString = searchString;  
            var tagTreePosition = new Rect(xMargin, yMargin + searchBarHeight, position.width - xMargin * 2f, position.height - searchBarHeight - yMargin * 2f);
            tagTreeView.OnGUI(tagTreePosition);
        }

        private void OnDisable()
        {
            tagTreeView.Save();
            AssetsModificationListener.BeforeProjectSaved -= OnSavingProject;

            if (searchField != null)
            {
                searchField.downOrUpArrowKeyPressed -= tagTreeView.SetFocusAndEnsureSelectedItem;
            }
        } 
    }
}
