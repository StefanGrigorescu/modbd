using MODBD_Common.Abstractions;

namespace MODBD_Common.Collections;

public class AppImmutableList<TItem> :
    AppReadOnlyList<TItem>,
    IReadOnlyList<TItem>,
    IEquatable<AppImmutableList<TItem>>,
    IImmutable,
    IImmutable<AppImmutableList<TItem>>
    where TItem : IImmutable
{
    /// <summary>
    /// Must be set in every constructor of this class!
    /// </summary>
    public int HashCode { get; private init; }

    public virtual bool Equals(AppImmutableList<TItem>? other) =>
        other is not null &&
        GetType() == other.GetType() &&
        this.SequenceEqual(other);

    public override bool Equals(object? obj) =>
        obj is AppImmutableList<TItem> other &&
        ((IEquatable<AppImmutableList<TItem>>)this).Equals(other);

    public override int GetHashCode() => HashCode;

    public static readonly new AppImmutableList<TItem> Empty = new();
    public static new AppImmutableList<TItem> New(TItem singleItem) => new([singleItem]);
    public static new AppImmutableList<TItem> New(IList<TItem> list) => list.IsNullOrEmpty() ? Empty : new(list);

    protected AppImmutableList() : base(Array.Empty<TItem>()) { HashCode = this.IReadOnlyListHashCode(); }
    protected AppImmutableList(IList<TItem> list) : base(list) { HashCode = this.IReadOnlyListHashCode(); }
}


public static class AppImmutableListFactory
{
    public static AppImmutableList<TOut> ToAppImmutableList<TIn, TOut>(
        this IEnumerable<TIn>? source,
        Func<TIn, TOut> to
    )
        where TIn : IImmutable
        where TOut : IImmutable
    {
        return source?
            .Select(to)
            .ToArray()
            .ToAppImmutableList() ??
            AppImmutableList<TOut>.Empty;
    }

    public static AppImmutableList<TItem> ToAppImmutableList<TItem>(this IList<TItem> list)
        where TItem : IImmutable =>
        AppImmutableList<TItem>.New(list);
}
