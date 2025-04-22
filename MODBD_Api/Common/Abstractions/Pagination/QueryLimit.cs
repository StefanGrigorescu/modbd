namespace MODBD_Api.Common.Abstractions.Pagination;

public sealed class QueryLimit : ValueObject<int>
{
    /// <summary>
    /// 100.
    /// </summary>
    public static readonly QueryLimit DefaultMaxItemsCount = new() { Value = 100, };
    /// <summary>
    /// 25.
    /// </summary>
    public static readonly QueryLimit SmallMaxItemsCount = new() { Value = 25, };
    /// <summary>
    /// 100.
    /// </summary>
    public static readonly QueryLimit PageSize = new() { Value = 100, };

    private QueryLimit() {  }
}
