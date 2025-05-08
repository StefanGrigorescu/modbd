using System.Diagnostics;

namespace MODBD_Common.Abstractions.Entities;

public abstract class EntityId<TValue> : 
    ValueObject<TValue>,
    IImmutable
    where TValue : IEquatable<TValue>, IComparable<TValue>
{
    public override string ToString() =>
        Value.ToString() ??
        throw new UnreachableException($"Id value was null or {nameof(EntityId<TValue>)} {nameof(ToString)} implementation was not provided by the {nameof(TValue)} type.");

    protected EntityId() : base() { }
}
