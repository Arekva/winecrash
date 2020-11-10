using System.Collections.Concurrent;
using System.Collections.Generic;

namespace WEngine
{
    public static class EConcurrentBag
    {
        public static void AddRange<T>(this ConcurrentBag<T> bag, IEnumerable<T> collection)
        {
            foreach (T element in collection)
            {
                bag.Add(element);
            }
        }
        
        public static void AddRange<T>(this ConcurrentBag<T> bag, params T[] collection)
        {
            foreach (T element in collection)
            {
                bag.Add(element);
            }
        }
    }
}