using System.Diagnostics.CodeAnalysis;

namespace MODBD_Common.Collections;

public static class CollectionsExtensions
{
    /// <summary>
    /// This method counts the total number of items in a collection. <br></br>
    /// If the collection is null or empty it returns 0. <br></br>
    /// Otherwise, it returns its items count. 
    /// </summary>
    /// <param name="collection">The collection of items.</param>
    /// <typeparam name="T">The type of the items in the collection.</typeparam>
    /// <returns>A number greater or equal to 0, representing the total number of items in the collection.</returns>
    public static int CountItems<T>(this IEnumerable<T> collection) =>
        collection.IsNullOrEmpty() ? 0 : collection.Count();

    /// <summary>
    /// This method counts the total number of items in a collection. <br></br>
    /// If the collection is null or empty it returns 0. <br></br>
    /// Otherwise, it returns its items count. 
    /// </summary>
    /// <param name="collection">The collection of items.</param>
    /// <typeparam name="T">The type of the items in the collection.</typeparam>
    /// <returns>A number greater or equal to 0, representing the total number of items in the collection.</returns>
    public static int CountItems<T>(this IReadOnlyList<T> collection) =>
        collection.IsNullOrEmpty() ? 0 : collection.Count;

    /// <summary>
    /// This method checks wether a collection has duplicate items. <br></br>
    /// If the collection is null or empty it returns false. <br></br>
    /// Otherwise, it uses the <typeparamref name="T"/> type's Equality method 
    /// to check wether there are duplicated items in the collection or not. 
    /// </summary>
    /// <param name="collection">The collection of items.</param>
    /// <typeparam name="T">The type of the items in the collection.</typeparam>
    /// <returns></returns>
    public static bool HasDuplicatedItems<T>(this IEnumerable<T> collection)
    {
        if (collection.IsNullOrEmpty())
        {
            return false;
        }

        HashSet<T> uniqueItems = [];
        foreach (T item in collection)
        {
            // If the element is not added to the set,
            // it means it is already present in the set and we return true.
            if (!uniqueItems.Add(item))
            {
                return true;
            }
        }

        return false;
    }

    public static bool DictionaryAndHashSetKeysMatch<TKey, TValue>(
        this IReadOnlyDictionary<TKey, TValue> dict,
        HashSet<TKey> set)
        where TKey : IEquatable<TKey>
    {
        if (dict.IsNullOrEmpty())
        {
            return set.SetEquals(new HashSet<TKey>());
        }

        HashSet<TKey>? dictKeys = dict
            .Select(kvp => kvp.Key)
            .ToHashSet();

        return set.SetEquals(dictKeys);
    }

    public static IDictionary<TKey, TValue> AddKvp<TKey, TValue>(this IDictionary<TKey, TValue> dict, KeyValuePair<TKey, TValue> kvp)
    {
        dict.Add(kvp.Key, kvp.Value);
        return dict;
    }

    public static bool IReadOnlyListEqual<T>(this IReadOnlyList<T>? left, IReadOnlyList<T>? right)
    {
        if ((object?)left is null && (object?)right is null)
        {
            return true;
        }

        if ((object?)left is null || (object?)right is null)
        {
            return false;
        }

        return left.SequenceEqual(right);
    }

    public static bool IReadOnlyDictionaryEqual<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue>? left, IReadOnlyDictionary<TKey, TValue>? right)
    {
        if ((object?)left is null && (object?)right is null)
        {
            return true;
        }

        if ((object?)left is null || (object?)right is null)
        {
            return false;
        }

        return left.SequenceEqual(right);
    }

    public static int IReadOnlyListHashCode<T>(this IReadOnlyList<T> collection) =>
        collection.Aggregate(
            0,
            (acc, crt) => HashCode.Combine(acc, crt?.GetHashCode())
        );

    public static int IReadOnlyDictionaryHashCode<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> collection) =>
        collection.Aggregate(
            0,
            (acc, crt) => HashCode.Combine(acc, crt.GetHashCode())
        );

    public static bool IsACollectionType(this Type type)
    {
        if (type == typeof(string))
        {
            return false;
        }

        if (type == typeof(Array) ||
            type.IsArray ||
            type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>) ||
            type.IsAssignableTo(typeof(IEnumerable<>))
        )
        {
            return true;
        }

        Type[] interfaces = type.GetInterfaces();

        return interfaces.Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>)) ||
            interfaces.Any(i => i.IsAssignableTo(typeof(IEnumerable<>))) ||
            (type.BaseType?.IsACollectionType() ?? false);
    }

    /// <summary>
    /// Checks whether <paramref name="collection"/> is null or empty.
    /// </summary>
    /// <typeparam name="T">The type of the <paramref name="collection"/>.</typeparam>
    /// <param name="collection">The <see cref="IEnumerable{T}"/> to be checked.</param>
    /// <returns>True if <paramref name="collection"/> is null or empty, false otherwise.</returns>
    public static bool IsNullOrEmpty<T>(
        [NotNullWhen(false)] this IEnumerable<T>? collection
    ) =>
        collection is null || !collection.Any();
}
