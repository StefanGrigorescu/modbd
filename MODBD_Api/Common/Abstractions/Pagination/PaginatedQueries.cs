using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using MODBD_Api.Common.Abstractions.Entities;

namespace MODBD_Api.Common.Abstractions.Pagination;

/// <summary>
/// Defaults: 
/// <list type="bullet">
/// <item><description><see cref="SortingDirection"/> defaults to <see cref="SortingDirection.Descending"/></description></item>
/// <item><description><see cref="SortBy"/> defaults to <see cref="SortEntities{TEntity}.LastInteractedOn"/></description></item>
/// <item><description><see cref="PreviousFilter"/> defaults to <see langword="null"/></description></item>
/// <item><description><see cref="PageSize"/> defaults to <see cref="QueryLimit.SmallMaxItemsCount"/> (<u>25</u>) for the first fetch (when <see cref="PreviousFilter"/> <u>is null</u>). <br></br>
/// For subsequent fetches  it defaults to <see cref="QueryLimit.PageSize"/>(<u>100</u>) (when <see cref="PreviousFilter"/> <u>is not null</u>).</description></item>
/// </list>
/// /// </summary>
/// <typeparam name="TEntity"></typeparam>
public abstract record PaginatedEntitiesQuery<TEntity>
    where TEntity : IEntity
{
    public required SortingDirection SortingDirection { get; init; } = SortingDirection.Descending;
    public required Expression<Func<TEntity, DateTime>> SortBy { get; init; } = SortEntities<TEntity>.ByDefault;
    public required DateTime? PreviousFilter { get; init; } = null;
    public required QueryLimit PageSize { get; init; } = QueryLimit.PageSize;

    [SetsRequiredMembers]
    protected PaginatedEntitiesQuery() 
    {
        // For the first fetch, query a smaller amount of items.
        PageSize = PreviousFilter is null ? 
            QueryLimit.SmallMaxItemsCount : 
            PageSize;
    }
}


/// <summary>
/// Defaults: 
/// <list type="bullet">
/// <item><description><see cref="SortingDirection"/> defaults to <see cref="SortingDirection.Descending"/></description></item>
/// <item><description><see cref="SortBy"/> defaults to <see cref="SortArchivedEntities{TArchivedEntity}.LastInteractedOn"/></description></item>
/// <item><description><see cref="PreviousFilter"/> defaults to <see langword="null"/></description></item>
/// <item><description><see cref="PageSize"/> defaults to <see cref="QueryLimit.SmallMaxItemsCount"/> (<u>25</u>) for the first fetch (when <see cref="PreviousFilter"/> <u>is null</u>). <br></br>
/// For subsequent fetches  it defaults to <see cref="QueryLimit.PageSize"/>(<u>100</u>) (when <see cref="PreviousFilter"/> <u>is not null</u>).</description></item>
/// </list>
/// /// </summary>
/// <typeparam name="TArchivedEntity"></typeparam>
public abstract record PaginatedArchivedEntitiesQuery<TArchivedEntity>
    where TArchivedEntity : ArchivedEntity
{
    public required SortingDirection SortingDirection { get; init; } = SortingDirection.Default;
    public required Expression<Func<TArchivedEntity, DateTime>> SortBy { get; init; } = SortArchivedEntities<TArchivedEntity>.ByDefault;
    public required DateTime? PreviousFilter { get; init; } = null;
    public required QueryLimit PageSize { get; init; } = QueryLimit.PageSize;

    [SetsRequiredMembers]
    protected PaginatedArchivedEntitiesQuery()
    {
        // For the first fetch, query a smaller amount of items.
        PageSize = PreviousFilter is null ?
            QueryLimit.SmallMaxItemsCount :
            PageSize;
    }
}
