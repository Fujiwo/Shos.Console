using System.Collections;

namespace Shos.Console;

public static class EnumerableExtensions
{
    public static int Count(this IEnumerable @this)
    {
        if (@this is ICollection collection)
            return collection.Count;

        var count = 0;
        foreach (var _ in @this)
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
        foreach (var element in @this)
            action(element);
    }

    public static void ForEach<TElement>(this IEnumerable<TElement> @this, Action<int, TElement> action)
    {
        var index = 0;
        foreach (var element in @this)
            action(index++, element);
    }
}
