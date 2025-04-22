using MODBD_Api.Common.Abstractions.Entities;
using MODBD_Api.Common.Collections;

namespace MODBD_Api.Common.Abstractions.DomainExceptions;

public class DuplicatedIdsException : ValueObjectException
{
    public static void ThrowIfHasDuplicatedIds<TEntity, TEntityId, TIdValue>(
        IReadOnlyList<TEntity> collection,
        string collectionTypeName
    )
        where TEntityId : EntityId<TIdValue>
        where TIdValue : IEquatable<TIdValue>, IComparable<TIdValue>
        where TEntity : Entity<TEntityId, TIdValue>
    {
        if (collection
                .Select(kvp => kvp.Id)
                .HasDuplicatedItems())
        {
            throw new DuplicatedIdsException(collectionTypeName, typeof(TEntity).Name);
        }
    }

    private DuplicatedIdsException(string collectionTypeName, string itemsTypeName) : base(
        $"{collectionTypeName} must not have {itemsTypeName} items with duplicated names!.") { }
}
