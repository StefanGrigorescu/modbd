using System.Linq.Expressions;
using MODBD_Api.Common.Abstractions.Entities;

namespace MODBD_Api.Common.Abstractions.Pagination;

public static class PaginateEntities
{
    /// <summary>
    /// Applies pagination to <paramref name="query"/> based on the <paramref name="pagination"/> parameters.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="query"></param>
    /// <param name="pagination"></param>
    /// <returns></returns>
    public static IQueryable<TEntity> Paginate<TEntity>(
        this IQueryable<TEntity> query,
        PaginatedEntitiesQuery<TEntity> pagination
    ) where TEntity : IEntity =>
        query
            .SkipPreviousResults(pagination)
            .Sort(pagination)
            .Take(pagination.PageSize);

    /// <summary>
    /// Cursor pagination: skips the results that were already fetched in the previous request.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="query"></param>
    /// <param name="pagination"></param>
    /// <returns></returns>
    private static IQueryable<TEntity> SkipPreviousResults<TEntity>(
        this IQueryable<TEntity> query,
        PaginatedEntitiesQuery<TEntity> pagination
    ) where TEntity : IEntity
    {
        if(pagination.PreviousFilter is null)
        {
            return query;
        }

        // Extract the property to sort by from the SortBy expression
        ParameterExpression entity = Expression.Parameter(typeof(TEntity), "entity");
        InvocationExpression entityProperty = Expression.Invoke(pagination.SortBy, entity);
        ConstantExpression previousFilter = Expression.Constant(pagination.PreviousFilter.Value, typeof(DateTime));

        // Construct the predicate dynamically
        Expression predicate = pagination.SortingDirection == SortingDirection.Ascending  ?
            Expression.GreaterThan(entityProperty, previousFilter) :
            Expression.LessThan(entityProperty, previousFilter);

        Expression<Func<TEntity, bool>> lambda = Expression.Lambda<Func<TEntity, bool>>(predicate, entity);

        return query.Where(lambda);
    }

    private static IOrderedQueryable<TEntity> Sort<TEntity>(
        this IQueryable<TEntity> query,
        PaginatedEntitiesQuery<TEntity> pagination
    ) where TEntity : IEntity
    {
        return pagination.SortingDirection == SortingDirection.Ascending ?
            query.OrderBy(pagination.SortBy) :
            query.OrderByDescending(pagination.SortBy);
    }
}


public static class PaginateArchivedEntities
{
    /// <summary>
    /// Applies pagination to <paramref name="query"/> based on the <paramref name="pagination"/> parameters.
    /// </summary>
    /// <typeparam name="TArchivedEntity"></typeparam>
    /// <param name="query"></param>
    /// <param name="pagination"></param>
    /// <returns></returns>
    public static IQueryable<TArchivedEntity> Paginate<TArchivedEntity>(
        this IQueryable<TArchivedEntity> query,
        PaginatedArchivedEntitiesQuery<TArchivedEntity> pagination
    ) where TArchivedEntity : ArchivedEntity =>
        query
            .SkipPreviousResults(pagination)
            .Sort(pagination)
            .Take(pagination.PageSize);

    /// <summary>
    /// Cursor pagination: skips the results that were already fetched in the previous request.
    /// </summary>
    /// <typeparam name="TArchivedEntity"></typeparam>
    /// <param name="query"></param>
    /// <param name="pagination"></param>
    /// <returns></returns>
    private static IQueryable<TArchivedEntity> SkipPreviousResults<TArchivedEntity>(
        this IQueryable<TArchivedEntity> query,
        PaginatedArchivedEntitiesQuery<TArchivedEntity> pagination
    ) where TArchivedEntity : ArchivedEntity
    {
        if (pagination.PreviousFilter is null)
        {
            return query;
        }

        // Extract the property to sort by from the SortBy expression
        ParameterExpression entity = Expression.Parameter(typeof(TArchivedEntity), "entity");
        InvocationExpression entityProperty = Expression.Invoke(pagination.SortBy, entity);
        ConstantExpression previousFilter = Expression.Constant(pagination.PreviousFilter.Value, typeof(DateTime));

        // Construct the predicate dynamically
        Expression predicate = pagination.SortingDirection == SortingDirection.Ascending ?
            Expression.GreaterThan(entityProperty, previousFilter) :
            Expression.LessThan(entityProperty, previousFilter);

        Expression<Func<TArchivedEntity, bool>> lambda = Expression.Lambda<Func<TArchivedEntity, bool>>(predicate, entity);

        return query.Where(lambda);
    }

    private static IOrderedQueryable<TArchivedEntity> Sort<TArchivedEntity>(
        this IQueryable<TArchivedEntity> query,
        PaginatedArchivedEntitiesQuery<TArchivedEntity> pagination
    ) where TArchivedEntity : ArchivedEntity
    {
        return pagination.SortingDirection == SortingDirection.Ascending ?
            query.OrderBy(pagination.SortBy) :
            query.OrderByDescending(pagination.SortBy);
    }
}
