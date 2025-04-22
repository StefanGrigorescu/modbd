using System.Linq.Expressions;
using MODBD_Api.Common.Abstractions.DiscriminatedUnions;
using MODBD_Api.Common.Abstractions.Entities;

namespace MODBD_Api.Common.Abstractions.Pagination;

public static class SortEntities<TEntity>
    where TEntity : IEntity
{
    public static Expression<Func<TEntity, DateTime>> By(
        EntitiesSortingCriteriaRequest sortingCriteria
    ) =>
        sortingCriteria switch
        {
            EntitiesSortingCriteriaRequest.LastInteractedOn => LastInteractedOn,
            EntitiesSortingCriteriaRequest.CreatedOn => CreatedOn,
            _ => throw SortingCriteriaUndefined.Exception,
        };

    /// <summary>
    /// Defaults to <see cref="LastInteractedOn"/>.
    /// </summary>
    public static Expression<Func<TEntity, DateTime>> ByDefault =>
        LastInteractedOn;

    public static readonly Expression<Func<TEntity, DateTime>> LastInteractedOn =
        entity => entity.LastUpdatedOn ?? entity.CreatedOn;

    public static readonly Expression<Func<TEntity, DateTime>> CreatedOn =
        entity => entity.CreatedOn;
}

public enum EntitiesSortingCriteriaRequest
{
    LastInteractedOn = 0,
    CreatedOn = 1,
}


public static class SortArchivedEntities<TArchivedEntity>
    where TArchivedEntity : ArchivedEntity
{
    public static Expression<Func<TArchivedEntity, DateTime>> By(
        ArchivedEntitiesSortingCriteriaRequest sortingCriteria
    ) =>
        sortingCriteria switch
        {
            ArchivedEntitiesSortingCriteriaRequest.ArchivedOn => ArchivedOn,
            ArchivedEntitiesSortingCriteriaRequest.CreatedOn => CreatedOn,
            _ => throw SortingCriteriaUndefined.Exception,
        };

    /// <summary>
    /// Defaults to <see cref="ArchivedOn"/>.
    /// </summary>
    public static Expression<Func<TArchivedEntity, DateTime>> ByDefault =>
        ArchivedOn;

    public static readonly Expression<Func<TArchivedEntity, DateTime>> ArchivedOn =
        entity => entity.ArchivedOn;

    public static readonly Expression<Func<TArchivedEntity, DateTime>> CreatedOn =
        entity => entity.CreatedOn;
}

public enum ArchivedEntitiesSortingCriteriaRequest
{
    ArchivedOn = 0,
    CreatedOn = 1,
}


public static class SortingCriteriaUndefined
{
    public static readonly TypeUndefinedException Exception = TypeUndefinedException.FromCustomMessage(
        "Specified order by criteria is not valid for this type of item."
    );
}
