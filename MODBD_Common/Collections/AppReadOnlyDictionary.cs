using System.Collections.ObjectModel;
using MODBD_Common.Abstractions;

namespace MODBD_Common.Collections;

public class AppReadOnlyDictionary<TKey, TValue> :
    ReadOnlyDictionary<TKey, TValue>,
    IReadOnlyDictionary<TKey, TValue>,
    IEquatable<AppReadOnlyDictionary<TKey, TValue>>,
    IImmutable,
    IImmutable<AppReadOnlyDictionary<TKey, TValue>>
    where TKey : notnull
{
    public virtual bool Equals(AppReadOnlyDictionary<TKey, TValue>? other) =>
        other is not null &&
        GetType() == other.GetType() &&
        this.SequenceEqual(other);

    public virtual bool Equals(IReadOnlyDictionary<TKey, TValue>? other) =>
        other is not null &&
        GetType() == other.GetType() &&
        this.SequenceEqual(other);

    public override bool Equals(object? obj) =>
        obj is AppReadOnlyDictionary<TKey, TValue> other &&
        ((IEquatable<AppReadOnlyDictionary<TKey, TValue>>)this).Equals(other);

    public static bool operator ==(AppReadOnlyDictionary<TKey, TValue>? left, AppReadOnlyDictionary<TKey, TValue>? right) =>
        (left is null && right is null) || (
            left is not null &&
            right is not null &&
            left.Equals(right)
        );

    public static bool operator !=(AppReadOnlyDictionary<TKey, TValue>? left, AppReadOnlyDictionary<TKey, TValue>? right) =>
        !(left == right);

    public override int GetHashCode() => this.IReadOnlyDictionaryHashCode();

    public static readonly new AppReadOnlyDictionary<TKey, TValue> Empty = new();
    public static AppReadOnlyDictionary<TKey, TValue> New(TKey key, TValue value) => new (key, value);
    public static AppReadOnlyDictionary<TKey, TValue> New(KeyValuePair<TKey, TValue> kvp) => new (kvp);
    public static AppReadOnlyDictionary<TKey, TValue> New(IDictionary<TKey, TValue> dictionary) => dictionary.IsNullOrEmpty() ? Empty : new (dictionary);

    protected AppReadOnlyDictionary() : base(new Dictionary<TKey, TValue>()) { }
    protected AppReadOnlyDictionary(TKey key, TValue value) : base(new Dictionary<TKey, TValue>( [new (key, value)] )) { }
    protected AppReadOnlyDictionary(KeyValuePair<TKey, TValue> kvp) : base(new Dictionary<TKey, TValue>([kvp])) { }
    protected AppReadOnlyDictionary(IDictionary<TKey, TValue> dictionary) : base(dictionary) { }
}


public static class AppReadOnlyDictionaryFactory
{
    public static AppReadOnlyDictionary<TKey, TValue> ToAppReadOnlyDictionary<TSource, TKey, TValue>(
        this IEnumerable<TSource>? source,
        Func<TSource, TKey> keySelector,
        Func<TSource, TValue> valueSelector
    )
        where TKey : notnull
    {
        return source?
            .ToDictionary(keySelector, valueSelector, null)
            .ToAppReadOnlyDictionary() ??
            AppReadOnlyDictionary<TKey, TValue>.Empty;
    }

    public static AppReadOnlyDictionary<TKey, TValue> ToAppReadOnlyDictionary<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
        where TKey : notnull =>
        AppReadOnlyDictionary<TKey, TValue>.New(dictionary);
}
