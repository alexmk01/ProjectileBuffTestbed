using System;

namespace Common.Tags
{
    [Serializable]
    public struct TagCount<T> : IEquatable<TagCount<T>>, IComparable<TagCount<T>> where T : ITagReference
    {
        public static TagCount<T> operator +(TagCount<T> tagCount, int amount) 
        {
            return new TagCount<T> { tagReference = tagCount.tagReference, count = tagCount.count + amount };
        }

        public static TagCount<T> operator -(TagCount<T> tagCount, int amount)
        {
            return tagCount + (-amount);
        }

        public T tagReference;
        public int count;

        public bool Add(in Tag tag, int amount) 
        { 
            if (tagReference.GetValue() == tag)
            {
                count += amount;
                return true;
            }

            return false;
        }

        public bool Equals(TagCount<T> other)
        {
            return count == other.count && tagReference.GetValue() == other.tagReference.GetValue();
        }

        public override bool Equals(object obj)
        {
            return obj is TagCount<T> tagCount && Equals(tagCount);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(tagReference.GetValue(), count);
        }

        public int CompareTo(TagCount<T> other) => count.CompareTo(other.count);
    }
}
