using MODBD_Common.Abstractions;

namespace MODBD_Common.Collections;

public class AppImmutableDictionary<TKey, TValue> :
    AppReadOnlyDictionary<TKey, TValue>,
    IReadOnlyDictionary<TKey, TValue>,
    IEquatable<AppImmutableDictionary<TKey, TValue>>,
    IImmutable,
    IImmutable<AppImmutableDictionary<TKey, TValue>>
    where TKey : notnull, IImmutable
    where TValue : IImmutable
{
    /// <summary>
    /// Must be set in every constructor of this class!
    /// </summary>
    public int HashCode { get; private init; }

    public virtual bool Equals(AppImmutableDictionary<TKey, TValue>? other) =>
        other is not null &&
        GetType() == other.GetType() &&
        this.SequenceEqual(other);

    public override bool Equals(object? obj) =>
        obj is AppImmutableDictionary<TKey, TValue> other &&
        ((IEquatable<AppImmutableDictionary<TKey, TValue>>)this).Equals(other);

    public override int GetHashCode() => HashCode;

    public static readonly new AppImmutableDictionary<TKey, TValue> Empty = new();
    public static new AppImmutableDictionary<TKey, TValue> New(TKey key, TValue value) => new(key, value);
    public static new AppImmutableDictionary<TKey, TValue> New(KeyValuePair<TKey, TValue> kvp) => new(kvp);
    public static new AppImmutableDictionary<TKey, TValue> New(IDictionary<TKey, TValue> dictionary) => dictionary.IsNullOrEmpty() ? Empty : new(dictionary);

    protected AppImmutableDictionary() : base(new Dictionary<TKey, TValue>()) { HashCode = this.IReadOnlyDictionaryHashCode(); }
    protected AppImmutableDictionary(TKey key, TValue value) : base(new Dictionary<TKey, TValue>([new(key, value)])) { HashCode = this.IReadOnlyDictionaryHashCode(); }
    protected AppImmutableDictionary(KeyValuePair<TKey, TValue> kvp) : base(new Dictionary<TKey, TValue>([kvp])) { HashCode = this.IReadOnlyDictionaryHashCode(); }
    protected AppImmutableDictionary(IDictionary<TKey, TValue> dictionary) : base(dictionary) { HashCode = this.IReadOnlyDictionaryHashCode(); }
}


public static class AppImmutableDictionaryFactory
{
    public static AppImmutableDictionary<TKey, TValue> ToAppImmutableDictionary<TSource, TKey, TValue>(
        this IEnumerable<TSource>? source,
        Func<TSource, TKey> keySelector,
        Func<TSource, TValue> valueSelector
    )
    where TKey : notnull, IImmutable
    where TValue : IImmutable
    {
        return source?
            .ToDictionary(keySelector, valueSelector, null)
            .ToAppImmutableDictionary() ??
            AppImmutableDictionary<TKey, TValue>.Empty;
    }

    public static AppImmutableDictionary<TKey, TValue> ToAppImmutableDictionary<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
        where TKey : notnull, IImmutable
        where TValue : IImmutable =>
        AppImmutableDictionary<TKey, TValue>.New(dictionary);
}
