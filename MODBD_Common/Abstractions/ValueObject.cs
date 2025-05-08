using System.Diagnostics;
using MODBD_Common.NullableTypes;

namespace MODBD_Common.Abstractions;

[DebuggerDisplay("{Value}")]
public abstract class ValueObject<TValue> : 
    IEquatable<ValueObject<TValue>>, 
    IEquatable<TValue>, 
    IImmutable<ValueObject<TValue>>,
    IImmutable<TValue>,
    IComparable<ValueObject<TValue>>,
    IComparable<TValue>
    where TValue : IEquatable<TValue>, IComparable<TValue>
{
    public required TValue Value { get; init; }

    public static implicit operator TValue(ValueObject<TValue> valueObject) => valueObject.Value;

    public bool Equals(ValueObject<TValue>? other) =>
        other is not null &&
        GetType() == other.GetType() &&
        EqualityComparer<TValue>.Default.Equals(Value, other.Value);

    public bool Equals(TValue? other) =>
        other is not null &&
        GetType() == other.GetType() &&
        EqualityComparer<TValue>.Default.Equals(Value, other);

    public override bool Equals(object? obj) =>
        obj is ValueObject<TValue> other &&
        ((IEquatable<ValueObject<TValue>>)this).Equals(other);

    public static bool operator ==(ValueObject<TValue>? left, ValueObject<TValue>? right) =>
        (left is null && right is null) || (
            left is not null && 
            right is not null && 
            left.Equals(right)
        );

    public static bool operator !=(ValueObject<TValue>? left, ValueObject<TValue>? right) =>
        !(left == right);

    public override int GetHashCode() =>
        EqualityComparer<TValue>.Default.GetHashCode(Value);

    public int CompareTo(ValueObject<TValue>? other) =>
        other is not null ?
            Value.CompareTo(other.Value) :
            CompareToResult.WhenOtherIsNull;

    public int CompareTo(TValue? other) =>
        other is not null ?
            Value.CompareTo(other) :
            CompareToResult.WhenOtherIsNull;

    public override string ToString() =>
        Value?.ToString() ?? string.Empty;

    protected ValueObject() { }
}


public abstract class NullableValueObject<TValue> : 
    IEquatable<NullableValueObject<TValue>>,
    IEquatable<TValue>,
    IImmutable<NullableValueObject<TValue>>,
    IImmutable<TValue>,
    IComparable<NullableValueObject<TValue>>,
    IComparable<TValue>
    where TValue : IEquatable<TValue>, IComparable<TValue>
{
    public required TValue? Value { get; init; }

    public static implicit operator TValue?(NullableValueObject<TValue> valueObject) => valueObject.Value;

    public TOut Match<TOut>(
        Func<TValue, TOut> onNotNull,
        Func<TOut> onNull
    ) => Value switch
    {
        null => onNull(),
        _ => onNotNull(Value)
    };

    public bool Equals(NullableValueObject<TValue>? other) =>
        other is not null &&
        GetType() == other.GetType() &&
        EqualityComparer<TValue>.Default.Equals(Value, other.Value);

    public bool Equals(TValue? other) =>
        (other is null && 
        Value is null) ||
        (other is not null &&
        GetType() == other.GetType() &&
        EqualityComparer<TValue>.Default.Equals(Value, other));

    public override bool Equals(object? obj) =>
        obj is ValueObject<TValue> other &&
        ((IEquatable<ValueObject<TValue>>)this).Equals(other);

    public static bool operator ==(NullableValueObject<TValue>? left, NullableValueObject<TValue>? right) =>
        (left is null && right is null) || (
            left is not null &&
            right is not null &&
            left.Equals(right)
        );

    public static bool operator !=(NullableValueObject<TValue>? left, NullableValueObject<TValue>? right) =>
        !(left == right);

    public override int GetHashCode() =>
        EqualityComparer<TValue>.Default.GetHashCode(Value);

    public int CompareTo(NullableValueObject<TValue>? other)
    {
        if(other is null)
        {
            return CompareToResult.WhenOtherIsNull;
        }

        if(Value is null && other.Value is null)
        {
            return 0;
        }

        if(other.Value is null)
        {
            return CompareToResult.WhenOtherIsNull;
        }

        if(Value is null)
        {
            return CompareToResult.WhenThisIsNull;
        }

        return Value.CompareTo(other.Value);
    }

    public int CompareTo(TValue? other)
    {
        if (Value is null && other is null)
        {
            return 0;
        }

        if (other is null)
        {
            return CompareToResult.WhenOtherIsNull;
        }

        if (Value is null)
        {
            return CompareToResult.WhenThisIsNull;
        }

        return Value.CompareTo(other);
    }

    public override string ToString() =>
        Value?.ToString() ?? string.Empty;

    protected NullableValueObject() { }
}


/*
public abstract class ValueObject<TValue, TThis> 
    where TThis : ValueObject<TValue, TThis>
{
    private static readonly Func<TThis> _factory;

    public virtual TValue? Value { get; protected set; }

    protected virtual void Validate() { }

    protected virtual bool TryValidate() =>
        true;

    static ValueObject()
    {
        ConstructorInfo constructor = typeof(TThis).GetTypeInfo().DeclaredConstructors.First();
        Expression[] arguments = [];
        NewExpression body = Expression.New(constructor, arguments);
        _factory = (Func<TThis>)Expression.Lambda(typeof(Func<TThis>), body).Compile();
    }

    public static TThis From(TValue? item)
    {
        TThis val = _factory();
        val.Value = item;
        val.Validate();
        return val;
    }

    public static bool TryFrom(TValue? sourceItem, out TThis? responseValueObject)
    {
        TThis val = _factory();
        val.Value = sourceItem;
        responseValueObject = val.TryValidate() ? val : null;
        return responseValueObject is not null;
    }

    protected virtual bool Equals(ValueObject<TValue, TThis> other) => 
        EqualityComparer<TValue>.Default.Equals(Value, other.Value);

    public override bool Equals(object? obj)
    {
        if (obj is null)
        {
            return false;
        }

        if (this == obj)
        {
            return true;
        }

        if (obj.GetType() == GetType())
        {
            return Equals((ValueObject<TValue, TThis>)obj);
        }

        return false;
    }

    public override int GetHashCode() => 
        EqualityComparer<TValue>.Default.GetHashCode(Value);

    public static bool operator ==(ValueObject<TValue, TThis> a, ValueObject<TValue, TThis> b)
    {
        if ((object?)a is null && (object?)b is null)
        {
            return true;
        }

        if ((object?)a is null || (object?)b is null)
        {
            return false;
        }

        return a.Equals(b);
    }

    public static bool operator !=(ValueObject<TValue, TThis> a, ValueObject<TValue, TThis> b) =>
        !(a == b);

    public override string ToString() => 
        Value?.ToString() ?? string.Empty;
}
*/
