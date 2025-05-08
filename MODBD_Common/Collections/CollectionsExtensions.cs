using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using MODBD_Common.Abstractions.Responses;

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
        HashSet<TKey> set
    ) where TKey : IEquatable<TKey> =>
        dict.IsNullOrEmpty() ?
            set.IsNullOrEmpty() :
            set.SetEquals(
                dict.Select(kvp => kvp.Key)
            );

    public static IDictionary<TKey, TValue> AddKvp<TKey, TValue>(this IDictionary<TKey, TValue> dict, KeyValuePair<TKey, TValue> kvp)
    {
        dict.Add(kvp.Key, kvp.Value);
        return dict;
    }

    public static IDictionary<TKey, TValue> AddKvp<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue value)
    {
        dict.Add(key, value);
        return dict;
    }

    public static bool IReadOnlyListContainsSameElements<T>(this IReadOnlyList<T>? left, IReadOnlyList<T>? right) =>
        (left is null && right is null) || (
            left is not null &&
            right is not null &&
            left.Count == right.Count && // The collections have the same number of elements and
            !left.Any((T element) =>       // there is no element in the left collection 
                !right.Contains(element)   // that is not contained in the right collection too
            )
        );

    public static bool IReadOnlyListEqual<T>(this IReadOnlyList<T>? left, IReadOnlyList<T>? right) =>
        (left is null && right is null) || (
            left is not null &&
            right is not null &&
            left.SequenceEqual(right)
        );

    public static bool IReadOnlyDictionaryEqual<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue>? left, IReadOnlyDictionary<TKey, TValue>? right) =>
        (left is null && right is null) || (
            left is not null &&
            right is not null &&
            left.SequenceEqual(right)
        );

    public static bool IReadOnlySetEqual<TItem>(this IReadOnlySet<TItem>? left, IReadOnlySet<TItem>? right) =>
        (left is null && right is null) || (
            left is not null &&
            right is not null &&
            left.SetEquals(right)
        );

    public static int IReadOnlyListHashCode<T>(this IReadOnlyList<T> collection) =>
        collection.Aggregate(
            0,
            (int acc, T crt) => HashCode.Combine(acc, crt?.GetHashCode())
        );

    public static int IReadOnlyDictionaryHashCode<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> collection) =>
        collection.Aggregate(
            0,
            (int acc, KeyValuePair<TKey, TValue> crt) => HashCode.Combine(acc, crt.GetHashCode())
        );

    public static int IReadOnlySetHashCode<TItem>(this IReadOnlySet<TItem> collection) =>
        collection
            .OrderBy(item => item?.GetHashCode())
            .Aggregate(
                0,
                (int acc, TItem crt) => HashCode.Combine(acc, crt?.GetHashCode())
            );

    public static bool IsACollectionType(this Type type) =>
        (type != typeof(string)) && (
            type.IsArray ||
            type == typeof(Array) ||
            IsIEnumerableOrAssignableToIt(type) ||
            type
                .GetInterfaces()
                .Any(IsIEnumerableOrAssignableToIt) ||
            (type.BaseType?.IsACollectionType() ?? false)
        );

    private static bool IsIEnumerableOrAssignableToIt(Type type) =>
        (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>)) ||
        type.IsAssignableTo(typeof(IEnumerable<>));

    public static bool IsAMutableCollectionType(this Type type) =>
        (type != typeof(string)) && (
            type.IsArray ||
            IsInMutableCollectionTypesOrAssignableToAnyOfThem(type) ||
            type
                .GetInterfaces()
                .Any(IsInMutableCollectionTypesOrAssignableToAnyOfThem) ||
            (type.BaseType?.IsAMutableCollectionType() ?? false)
        );

    private static bool IsInMutableCollectionTypesOrAssignableToAnyOfThem(Type type)
    {
        if(type.IsGenericType)
        {
            Type genericTypeDefinition = type.GetGenericTypeDefinition();
            if(_mutableCollectionTypes.Contains(genericTypeDefinition))
            {
                return true;
            }
        }
        return _mutableCollectionTypes.Any(type.IsAssignableTo);
    }

    private static readonly AppReadOnlySet<Type> _mutableCollectionTypes = AppReadOnlySet<Type>.New([
        typeof(Collection< >),
        typeof(Array),
        typeof(List < >),
        typeof(Dictionary <, >),
        typeof(HashSet < >),
        typeof(Stack < >),
        typeof(Queue < >),
        typeof(SortedList <, >),
        typeof(SortedDictionary <, >),
        typeof(SortedSet < >),
    ]);

    //private static readonly AppReadOnlySet<Type> _immutableCollectionTypes = AppReadOnlySet<Type>.New([
    //    typeof(ReadOnlyCollection< >),
    //    typeof(ReadOnlyDictionary <, >),
    //    typeof(AppReadOnlyList<>),
    //    typeof(AppReadOnlyDictionary <, >),
    //    typeof(AppReadOnlySet < >),
    //    typeof(AppImmutableList<>),
    //    typeof(AppImmutableDictionary <, >),
    //    typeof(AppImmutableSet < >),
    //]);

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
