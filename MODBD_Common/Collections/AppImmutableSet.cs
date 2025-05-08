using MODBD_Common.Abstractions;

namespace MODBD_Common.Collections;

public class AppImmutableSet<TItem> :
    AppReadOnlySet<TItem>,
    ICollection<TItem>, 
    ISet<TItem>, 
    IReadOnlyCollection<TItem>, 
    IReadOnlySet<TItem>,
    IEquatable<AppImmutableSet<TItem>>,
    IImmutable,
    IImmutable<AppImmutableSet<TItem>>
    where TItem : IImmutable
{
    /// <summary>
    /// Must be set in every constructor of this class!
    /// </summary>
    public int HashCode { get; private init; }

    public virtual bool Equals(AppImmutableSet<TItem>? other) =>
        other is not null &&
        GetType() == other.GetType() &&
        SetEquals(other);

    public override bool Equals(object? obj) =>
        obj is AppImmutableSet<TItem> other &&
        ((IEquatable<AppImmutableSet<TItem>>)this).Equals(other);

    public override int GetHashCode() => HashCode;

    public static readonly new AppImmutableSet<TItem> Empty = new();
    public static new AppImmutableSet<TItem> New(IEqualityComparer<TItem>? comparer) => new(comparer);
    public static new AppImmutableSet<TItem> New(IEnumerable<TItem> collection) => collection.IsNullOrEmpty() ? Empty : new(collection);
    public static new AppImmutableSet<TItem> New(IEnumerable<TItem> collection, IEqualityComparer<TItem>? comparer) => new(collection, comparer);

    protected AppImmutableSet() : base() { HashCode = this.IReadOnlySetHashCode(); }
    protected AppImmutableSet(IEqualityComparer<TItem>? comparer) : base(comparer) { HashCode = this.IReadOnlySetHashCode(); }
    protected AppImmutableSet(IEnumerable<TItem> collection) : base(collection) { HashCode = this.IReadOnlySetHashCode(); }
    protected AppImmutableSet(IEnumerable<TItem> collection, IEqualityComparer<TItem>? comparer) : base(collection, comparer) { HashCode = this.IReadOnlySetHashCode(); }
}


public static class AppImmutableSetFactory
{
    public static AppImmutableSet<TItem> ToAppImmutableSet<TItem>(this IEnumerable<TItem> list)
        where TItem : IImmutable =>
        AppImmutableSet<TItem>.New(list);
}
