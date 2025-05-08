using System.Collections.ObjectModel;
using MODBD_Common.Abstractions;

namespace MODBD_Common.Collections;

public class AppReadOnlyList<TItem> :
    ReadOnlyCollection<TItem>,
    IReadOnlyList<TItem>,
    IEquatable<AppReadOnlyList<TItem>>,
    IImmutable,
    IImmutable<AppReadOnlyList<TItem>>
{
    public virtual bool Equals(AppReadOnlyList<TItem>? other) =>
        other is not null &&
        GetType() == other.GetType() &&
        this.SequenceEqual(other);

    public virtual bool Equals(IReadOnlyList<TItem>? other) =>
        other is not null &&
        GetType() == other.GetType() &&
        this.SequenceEqual(other);

    public override bool Equals(object? obj) =>
        obj is AppReadOnlyList<TItem> other &&
        ((IEquatable<AppReadOnlyList<TItem>>)this).Equals(other);

    public static bool operator ==(AppReadOnlyList<TItem>? left, AppReadOnlyList<TItem>? right) =>
        (left is null && right is null) || (
            left is not null &&
            right is not null &&
            left.Equals(right)
        );

    public static bool operator !=(AppReadOnlyList<TItem>? left, AppReadOnlyList<TItem>? right) =>
        !(left == right);

    public override int GetHashCode() => this.IReadOnlyListHashCode();

    public static readonly new AppReadOnlyList<TItem> Empty = new();
    public static AppReadOnlyList<TItem> New(TItem singleItem) => new([singleItem]);
    public static AppReadOnlyList<TItem> New(IList<TItem> list) => list.IsNullOrEmpty() ? Empty : new(list);

    protected AppReadOnlyList() : base(Array.Empty<TItem>()) { }
    protected AppReadOnlyList(IList<TItem> list) : base(list) { }
}


public static class AppReadOnlyListFactory
{
    public static AppReadOnlyList<TOut> ToAppReadOnlyList<TIn, TOut>(
        this IEnumerable<TIn>? source,
        Func<TIn, TOut> to
    ) {
        return source?
            .Select(to)
            .ToArray()
            .ToAppReadOnlyList() ??
            AppReadOnlyList<TOut>.Empty;
    }

    public static AppReadOnlyList<TItem> ToAppReadOnlyList<TItem>(this IList<TItem> list) =>
        AppReadOnlyList<TItem>.New(list);
}
