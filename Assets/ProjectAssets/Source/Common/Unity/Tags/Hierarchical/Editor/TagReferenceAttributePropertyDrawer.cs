using UnityEditor;

namespace Common.Unity.Tags.Hierarchical.Editor
{
    [CustomPropertyDrawer(typeof(TagReferenceAttribute))]
    public sealed class TagReferenceAttributePropertyDrawer : HierarchicalTagReferencePropertyDrawerBase
    {
        protected override string TagIdPropertyName => null;
    }
}
