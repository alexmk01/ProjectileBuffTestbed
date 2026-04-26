using Common.Tags;
using Common.Unity.Tags.Hierarchical;

namespace Game.Infrastructure.Localization
{
    public static class LocalizationUtility
    {
        public const string NameKey = "{0}_name";
        public const string DescriptionKey = "{0}_description";
        
        public static string GetNameKey(in Tag id) => string.Format(NameKey, HierarchicalTagUtility.GetLocalTag(id, convertToLowerCase: true));
        public static string GetDescriptionKey(in Tag id) => string.Format(DescriptionKey, HierarchicalTagUtility.GetLocalTag(id, convertToLowerCase: true));
    }
}