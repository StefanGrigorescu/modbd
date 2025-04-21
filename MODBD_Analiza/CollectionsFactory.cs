using System.Collections.ObjectModel;

namespace MODBD_Analiza;

/// <summary>
/// Factory class that provides methods to instantiate collections behind generic interfaces. 
/// </summary>
public static class CollectionsFactory
{
    public static ReadOnlyDictionary<TKey, TValue> EmptyReadOnlyDictionary<TKey, TValue>()
        where TKey : notnull
    {
        return new ReadOnlyDictionary<TKey, TValue>(new Dictionary<TKey, TValue>());
    }

    public static IReadOnlyDictionary<TKey, TValue> EmptyIReadOnlyDictionary<TKey, TValue>()
        where TKey : notnull
    {
        return new ReadOnlyDictionary<TKey, TValue>(new Dictionary<TKey, TValue>());
    }

    public static IReadOnlyDictionary<TKey, TValue> ToIReadOnlyDictionary<TSource, TKey, TValue>(
        this IEnumerable<TSource>? source,
        Func<TSource, TKey> keySelector,
        Func<TSource, TValue> valueSelector)
        where TKey : notnull
    {
        return source?
            .ToDictionary(keySelector, valueSelector, null)
            .AsReadOnly() ??
            EmptyIReadOnlyDictionary<TKey, TValue>();
    }

    public static IReadOnlyDictionary<TKey, TSource> ToIReadOnlyDictionary<TKey, TSource>(
        this IEnumerable<TSource>? source,
        Func<TSource, TKey> keySelector)
    where TKey : notnull
    {
        return source?
            .ToDictionary(keySelector)
            .AsReadOnly() ??
            EmptyIReadOnlyDictionary<TKey, TSource>();
    }

    public static IReadOnlyList<T> EmptyIReadOnlyList<T>()
    {
        return Array.Empty<T>()
            .AsReadOnly();
    }

    public static IReadOnlyList<T> IReadOnlyList<T>(params T[] items)
    {
        return items
            .AsReadOnly();
    }

    public static IReadOnlyList<TOut> ToIReadOnlyList<TIn, TOut>(
        this IEnumerable<TIn>? source,
        Func<TIn, TOut> to)
    {
        return source?
            .Select(to)
            .ToArray()
            .AsReadOnly() ??
            EmptyIReadOnlyList<TOut>();
    }

    public static IReadOnlyList<T> ToIReadOnlyList<T>(this IEnumerable<T>? source)
    {
        return source?
            .ToArray()
            .AsReadOnly() ??
            EmptyIReadOnlyList<T>();
    }

    public static IReadOnlyList<T> ToIReadOnlyList<T>(this IList<T>? source)
    {
        return source?
            .AsReadOnly() ??
            EmptyIReadOnlyList<T>();
    }

    public static IReadOnlyCollection<T> IReadOnlyCollection<T>(params T[] items)
    {
        return new ReadOnlyCollection<T>(items);
    }

    public static SortedList<TKey, TValue> ToSortedList<TKey, TValue>(
        this IReadOnlyList<TValue>? source,
        Func<TValue, TKey> keySelector
    )
        where TKey : notnull
    {
        SortedList<TKey, TValue> sortedList = [];
        if (source is null)
        {
            return sortedList;
        }

        foreach (TValue item in source)
        {
            TKey key = keySelector(item);
            sortedList[key] = item;
        }

        return sortedList;
    }

    public static SortedList<TKey, TValue> ToSortedList<TKey, TValue>(
        this IEnumerable<TValue>? source,
        Func<TValue, TKey> keySelector
    )
        where TKey : notnull
    {
        SortedList<TKey, TValue> sortedList = [];
        if (source is null)
        {
            return sortedList;
        }

        foreach (TValue item in source)
        {
            TKey key = keySelector(item);
            sortedList[key] = item;
        }

        return sortedList;
    }

    public static IReadOnlyList<T> Distinct<T>(this IEnumerable<T>? source)
    {
        HashSet<T> hashSet = source is null ? 
            [] :
            new HashSet<T>(source);

        return hashSet.ToIReadOnlyList();
    }

    public static IEnumerable<T> IEnumerable<T>(T singleItem)
    {
        return (new[] { singleItem })
            .AsEnumerable();
    }
}
