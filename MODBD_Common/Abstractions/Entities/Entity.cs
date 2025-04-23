using System.Diagnostics;

namespace MODBD_Common.Abstractions.Entities;

/// <summary>
/// Abstraction for entities. 
/// <para>
/// Its general purpose is to abstract other entities that can not extend <see cref="Entity"/> class.
/// </para>
/// </summary>
public interface IEntity
{
    DateTime CreatedOn { get; set; }
    DateTime? LastUpdatedOn { get; set; }

    /// <summary>
    /// For an entity that was updated at least once, <see cref="LastUpdatedOn"/> will always be more recent than <see cref="CreatedOn"/>. <br></br>
    /// If it was not updated, <see cref="CreatedOn"/> will be the only date available.
    /// </summary>
    DateTime LastInteractedOn =>
        LastUpdatedOn ?? CreatedOn;
}


/// <summary>
/// Base class for entities. 
/// </summary>
public abstract class Entity : IEntity
{
    public DateTime CreatedOn { get; set; }
    public DateTime? LastUpdatedOn { get; set; }

    /// <summary>
    /// For an entity that was updated at least once, <see cref="LastUpdatedOn"/> will always be more recent than <see cref="CreatedOn"/>. <br></br>
    /// If it was not updated, <see cref="CreatedOn"/> will be the only date available.
    /// </summary>
    public DateTime LastInteractedOn =>
        LastUpdatedOn ?? CreatedOn;
}


/// <summary>
/// Base class for entities, having id property specified. 
/// </summary>
/// <typeparam name="TEntityId">Entity id type.</typeparam>
/// <typeparam name="TEntityIdValue">Entity id inner <see cref="IEntityId{TValue}.Value"/> property type.</typeparam>
[DebuggerDisplay("{typeof(TEntityId).Name} #{Id}")]
public abstract class Entity<TEntityId, TEntityIdValue>
    : Entity, IEquatable<Entity<TEntityId, TEntityIdValue>>
    where TEntityId : EntityId<TEntityIdValue>
    where TEntityIdValue : IEquatable<TEntityIdValue>, IComparable<TEntityIdValue>
{
    /// <summary>
    /// Entity id of type <typeparamref name="TEntityId"/>.
    /// </summary>
    public required TEntityId Id { get; init; }

    public bool Equals(Entity<TEntityId, TEntityIdValue>? other) =>
        other is not null &&
        GetType() == other.GetType() &&
        Id.Equals(other.Id);

    public override bool Equals(object? obj) =>
        obj is not null &&
        obj is Entity<TEntityId, TEntityIdValue> other &&
        ((IEquatable<Entity<TEntityId, TEntityIdValue>>)this).Equals(other);

    public static bool operator ==(Entity<TEntityId, TEntityIdValue> left, Entity<TEntityId, TEntityIdValue> right)
    {
        if ((object?)left is null && (object?)right is null)
        {
            return true;
        }

        if ((object?)left is null || (object?)right is null)
        {
            return false;
        }

        return left.Equals(right);
    }

    public static bool operator !=(Entity<TEntityId, TEntityIdValue> left, Entity<TEntityId, TEntityIdValue> right) =>
        !(left == right);

    public override int GetHashCode() =>
        Id.GetHashCode();

    public override string ToString() =>
        $"{GetType().Name} #{Id}";
}
