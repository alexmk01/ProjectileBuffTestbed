using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Common.Reflection;

namespace Common
{
    public static class CollectionsExtensions
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> collection)
        {
            if (collection == null) return true;
            int? count = (collection as ICollection<T>)?.Count ?? (collection as IReadOnlyCollection<T>)?.Count;
            return count == 0 || collection.FirstOrDefault() == null;
        }

        public static bool SequenceEqual<T>(this IEnumerable<T> first, IEnumerable<T> second)
        {
            if (first != null && second != null) return Enumerable.SequenceEqual(first, second);
            return first == null && second == null;
        }

        public static void CopyTo<T>(this IList<T> sourceList, int sourceIndex, IList<T> destinationList, int destinationIndex, int length)
        {
            if (sourceList is T[] sourceArray && destinationList is T[] destinationArray)
            {
                Array.Copy(sourceArray, sourceIndex, destinationArray, destinationIndex, length);
                return;
            }

            if (sourceList == null) throw new ArgumentNullException(nameof(sourceList));
            if (destinationList == null) throw new ArgumentNullException(nameof(destinationList));

            for (int i = 0; i < length; i++)
            {
                destinationList[destinationIndex + i] = sourceList[sourceIndex + i];
            }
        }

        public static void CopyTo<T>(this IList<T> sourceList, IList<T> destinationList, int length)
        {
            sourceList.CopyTo(0, destinationList, 0, length);
        }

        public static void CopyTo<T>(this IList<T> sourceList, IList<T> destinationList)
        {
            sourceList.CopyTo(0, destinationList, 0, sourceList.Count);
        }

        public static T[] CloneArray<T>(this T[] array)
        {
            if (typeof(ICloneable).IsAssignableFrom(typeof(T))) 
            {
                T[] clone = new T[array.Length];

                for (int i = 0; i < array.Length; i++)
                {
                    clone[i] = (T)((ICloneable)array[i]).Clone();
                }

                return clone; 
            }

            return (T[])array.Clone();
        }
        
        public static int IndexOf<T>(this IReadOnlyList<T> list, T item)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));
            var comparer = EqualityComparer<T>.Default;

            for (int i = 0; i < list.Count; i++)
            {
                if (comparer.Equals(list[i], item)) return i;
            }

            return -1;
        }

        public static T Find<T>(this IList<T> list, Predicate<T> match)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));
            if (match == null) throw new ArgumentNullException(nameof(match));
            int count = list.Count;

            for (int i = 0; i < count; i++)
            {
                T item = list[i];
                if (match.Invoke(item)) return item;
            }

            return default;
        }

        public static int BinarySearch<T>(this IList<T> list, T value, int startIndex = 0, IComparer<T> comparer = null)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));
            comparer = comparer ?? Comparer<T>.Default;
            int maxIndex = list.Count - 1;
            int lower = startIndex < maxIndex ? startIndex : maxIndex;
            int upper = maxIndex;

            while (lower <= upper)
            {
                int middle = lower + (upper - lower) / 2;
                int comparisonResult = comparer.Compare(value, list[middle]);
                if (comparisonResult == 0) return middle;
                else if (comparisonResult < 0) upper = middle - 1;
                else lower = middle + 1;
            }

            return ~lower;
        }

        public static int FindClosestMin<T>(this IList<T> list, T value, int startIndex = 0, IComparer<T> comparer = null)
        {
            int index = list.BinarySearch(value, startIndex, comparer);

            if (index < 0)
            {
                int count = list.Count;
                index = ~index - 1;
                if (index < 0) index = 0;
                else if (index >= count) index = count - 1;
            }

            return index;
        }
        
        public static bool Add<T>(T item, ref T[] array, ref int itemCount, bool addUnique = false, int maxNewSize = -1, int initialCapacity = 16)
        {
            array ??= new T[Math.Max(4, initialCapacity)];

            if (addUnique)
            {
                EqualityComparer<T> comparer = EqualityComparer<T>.Default;

                for (int i = 0; i < itemCount; i++)
                {
                    if (comparer.Equals(array[i], item)) return false;
                }
            }
            
            if (itemCount >= array.Length)
            {
                int newSize = Math.Max(8, itemCount * 2);
                if (maxNewSize > 0 && newSize > maxNewSize) newSize = Math.Max(maxNewSize, array.Length + 1); 
                Array.Resize(ref array, newSize);
            }

            array[itemCount++] = item;
            return true;
        }

        public static bool Add<T>(T item, ref T[] array, bool addUnique = false)
        {
            int size = array.Length;
            return Add(item, ref array, ref size, addUnique, size + 1);
        }
        
        public static void RemoveSwapBack<T>(this List<T> list, int index)
        {
            int lastItemIndex = list.Count - 1;
            if (lastItemIndex == -1) return;
            if (index != lastItemIndex) list[index] = list[lastItemIndex];
            list.RemoveAt(lastItemIndex);
        }

        public static void RemoveSwapBack<T>(this T[] array, int index, ref int itemsCount)
        {
            int lastItemIndex = itemsCount - 1;
            if (lastItemIndex < 0) return;
            if (index != lastItemIndex) array[index] = array[lastItemIndex];
            array[lastItemIndex] = default;
            itemsCount = lastItemIndex;
        }
        
        public static void RemoveSwapBack<T>(this ref Span<T> span, int index)
        {
            int lastItemIndex = span.Length - 1;
            if (lastItemIndex == -1) return;
            if (index != lastItemIndex) span[index] = span[lastItemIndex];
            span = span[..lastItemIndex];
        }

        public static int RemoveAll<T>(this T[] array, Predicate<T> match, int arraySizeOverride = -1)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));
            int size = arraySizeOverride < 0 ? array.Length : arraySizeOverride;
            //the first free slot in items array
            int freeIndex = 0;
            //Find the first item which needs to be removed.
            while (freeIndex < size && !match(array[freeIndex])) freeIndex++;
            if (freeIndex >= size) return 0;
            int current = freeIndex + 1;

            while (current < size)
            {
                //Find the first item which needs to be kept.
                while (current < size && match(array[current])) current++;

                if (current < size)
                {
                    //copy item to the free slot.
                    array[freeIndex++] = array[current++];
                }
            }

            Array.Clear(array, freeIndex, size - freeIndex);
            return size - freeIndex;
        }
        
        private static readonly int ListItemsFieldOffset = FieldAccessor.GetFieldOffset(typeof(List<int>), "_items");
        private static readonly int ListCountFieldOffset = FieldAccessor.GetFieldOffset(typeof(List<int>), "_size");
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ref T[] GetListArray<T>(List<T> list) => ref FieldAccessor.GetFieldValueRef<T[]>(list, ListItemsFieldOffset);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SetListCount<T>(List<T> list, int newCount)
        {
            FieldAccessor.GetFieldValueRef<int>(list, ListCountFieldOffset) = newCount;
            DebugUtils.Assert(list.Count == newCount);
        }

        public static Span<T> AsSpan<T>(this List<T> list)
        {
            DebugUtils.Assert(list != null);
            int count = list.Count;
            return count != 0 ? new(GetListArray(list), 0, count) : Span<T>.Empty;
        }

        public static void CopyTo<T>(this ReadOnlySpan<T> source, List<T> destination)
        {
            DebugUtils.Assert(destination != null);

            if (source.Length > destination.Capacity)
            {
                destination.Capacity = source.Length;
            }

            source.CopyTo(GetListArray(destination));

            if (source.Length > destination.Count)
            {
                SetListCount(destination, source.Length);
            }
        }

        public static void AddItems<T>(this List<T> list, in ReadOnlySpan<T> items)
        {
            int newCount = list.Count + items.Length;

            if (newCount > list.Capacity)
            {
                list.Capacity = newCount * 2;
            }

            Span<T> destination = new(GetListArray(list), list.Count, items.Length);
            items.CopyTo(destination);
            SetListCount(list, newCount);
        }

        public static void AddItems<T>(this List<T> list, ICollection<T> items)
        {
            if (items.Count == 0) return;
            int newCount = list.Count + items.Count;

            if (newCount > list.Capacity)
            {
                list.Capacity = newCount * 2;
            }

            T[] listArray = GetListArray(list);
            DebugUtils.Assert(listArray.Length >= newCount);
            items.CopyTo(listArray, list.Count);
            SetListCount(list, newCount);
        }

        public static void AddItems<T>(this List<T> list, IReadOnlyCollection<T> items)
        {
            if (items is ICollection<T> itemsCollection) list.AddItems(itemsCollection);
            else list.AddRange(items);
        }
    }
}
