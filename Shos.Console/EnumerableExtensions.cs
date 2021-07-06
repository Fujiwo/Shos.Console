using System;
using System.Collections;
using System.Collections.Generic;

namespace Shos.Console
{
    public static class EnumerableExtensions
    {
        public static int Count(this IEnumerable @this)
        {
            var collection = @this as ICollection;
            if (collection is not null)
                return collection.Count;

            var count = 0;
            foreach (var element in @this)
                count++;
            return count;
        }

        public static IEnumerable<TResult> Select<TValue, TResult>(this IEnumerable @this, Func<TValue, TResult> predicate)
        {
            foreach (TValue element in @this)
                yield return predicate(element);
        }

        public static void ForEach<TElement>(this IEnumerable<TElement> @this, Action<TElement> action)
        {
            foreach (TElement element in @this)
                action(element);
        }

        public static void ForEach<TElement>(this IEnumerable<TElement> @this, Action<int, TElement> action)
        {
            var index = 0;
            foreach (TElement element in @this)
                action(index++, element);
        }
    }

}
