using MODBD_Common.Abstractions;

namespace MODBD_Common.Collections;

public class AppReadOnlySet<TItem> :
    HashSet<TItem>,
    ICollection<TItem>, 
    ISet<TItem>, 
    IReadOnlyCollection<TItem>, 
    IReadOnlySet<TItem>,
    IEquatable<AppReadOnlySet<TItem>>,
    IImmutable,
    IImmutable<AppReadOnlySet<TItem>>
{
    public virtual bool Equals(AppReadOnlySet<TItem>? other) =>
        other is not null &&
        GetType() == other.GetType() &&
        SetEquals(other);

    public virtual bool Equals(IReadOnlySet<TItem>? other) =>
        other is not null &&
        GetType() == other.GetType() &&
        SetEquals(other);

    public override bool Equals(object? obj) =>
        obj is AppReadOnlySet<TItem> other &&
        ((IEquatable<AppReadOnlySet<TItem>>)this).Equals(other);

    public static bool operator ==(AppReadOnlySet<TItem>? left, AppReadOnlySet<TItem>? right) =>
        (left is null && right is null) || (
            left is not null &&
            right is not null &&
            left.Equals(right)
        );

    public static bool operator !=(AppReadOnlySet<TItem>? left, AppReadOnlySet<TItem>? right) =>
        !(left == right);

    public override int GetHashCode() => this.IReadOnlySetHashCode();

    public static readonly AppReadOnlySet<TItem> Empty = new();
    public static AppReadOnlySet<TItem> New(IEqualityComparer<TItem>? comparer) => new(comparer);
    public static AppReadOnlySet<TItem> New(IEnumerable<TItem> collection) => collection.IsNullOrEmpty() ? Empty : new(collection);
    public static AppReadOnlySet<TItem> New(IEnumerable<TItem> collection, IEqualityComparer<TItem>? comparer) => new(collection, comparer);

    protected AppReadOnlySet() : base() { }
    protected AppReadOnlySet(IEqualityComparer<TItem>? comparer) : base(comparer) { }
    protected AppReadOnlySet(IEnumerable<TItem> collection) : base(collection) { }
    protected AppReadOnlySet(IEnumerable<TItem> collection, IEqualityComparer<TItem>? comparer) : base(collection, comparer) { }
}


public static class AppReadOnlySetFactory
{
    public static AppReadOnlySet<TItem> ToAppReadOnlySet<TItem>(this IEnumerable<TItem> list) =>
        AppReadOnlySet<TItem>.New(list);
}
