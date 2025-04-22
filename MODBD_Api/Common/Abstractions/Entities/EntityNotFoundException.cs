using MODBD_Api.Common.Abstractions.DomainExceptions;

namespace MODBD_Api.Common.Abstractions.Entities;

public sealed class EntityNotFoundException<TEntity> : DomainObjectException
{
    public EntityNotFoundException(object entityId) :
        base($"{typeof(TEntity).Name} with id {entityId} was not found!")
    { }

    public static string MessageFor(object entityId) => 
        $"{typeof(TEntity).Name} with id {entityId} was not found!";
}
