using MODBD_Common.Abstractions.Entities;
using System.Diagnostics.CodeAnalysis;

namespace MODBD_Common.Abstractions;

/// <summary>
/// Base class for archived entities.
/// </summary>
public abstract class ArchivedEntity
{
    public DateTime CreatedOn { get; init; }
    public DateTime ArchivedOn { get; set; }

    protected ArchivedEntity(Entity entity)
    {
        CreatedOn = entity.CreatedOn;
    }
    protected ArchivedEntity() : base() { }
}


/// <summary>
/// Base class for archived entities, having id property specified. 
/// </summary>
/// <typeparam name="TEntity">Entity type.</typeparam>
/// <typeparam name="TEntityId">Entity id type.</typeparam>
/// <typeparam name="TEntityIdValue">Entity id inner <see cref="IEntityId{TValue}.Value"/> property type.</typeparam>
public abstract class ArchivedEntity<TEntity, TEntityId, TEntityIdValue> : 
    ArchivedEntity, IEquatable<ArchivedEntity<TEntity, TEntityId, TEntityIdValue>>
    where TEntity : Entity<TEntityId, TEntityIdValue>
    where TEntityId : EntityId<TEntityIdValue>
    where TEntityIdValue : IEquatable<TEntityIdValue>, IComparable<TEntityIdValue>
{
    public required TEntityId Id { get; init; }

    public bool Equals(ArchivedEntity<TEntity, TEntityId, TEntityIdValue>? other) =>
        other is not null &&
        GetType() == other.GetType() &&
        Id.Equals(other.Id);

    public override bool Equals(object? obj) =>
        obj is ArchivedEntity<TEntity, TEntityId, TEntityIdValue> other &&
        ((IEquatable<ArchivedEntity<TEntity, TEntityId, TEntityIdValue>>)this).Equals(other);

    public static bool operator ==(ArchivedEntity<TEntity, TEntityId, TEntityIdValue> left, ArchivedEntity<TEntity, TEntityId, TEntityIdValue> right) =>
        (left is null && right is null) || (
            left is not null &&
            right is not null &&
            left.Equals(right)
        );

    public static bool operator !=(ArchivedEntity<TEntity, TEntityId, TEntityIdValue> left, ArchivedEntity<TEntity, TEntityId, TEntityIdValue> right) =>
        !(left == right);

    public override int GetHashCode() =>
        Id.GetHashCode();

    public override string ToString() =>
        $"{GetType().Name} #{Id}";

    [SetsRequiredMembers]
    protected ArchivedEntity(Entity<TEntityId, TEntityIdValue> entity) : base(entity)
    {
        Id = entity.Id;
        CreatedOn = entity.CreatedOn;
    }
    protected ArchivedEntity() : base() { }
}
