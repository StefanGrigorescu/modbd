using MODBD_Common.Abstractions;
using MODBD_Common.Abstractions.DiscriminatedUnions;
using MODBD_Common.NullableTypes;
using System.Numerics;

namespace MODBD_Common.NumericTypes.Ranges;

public interface IUpperBound<TNumber> : IComparable<IUpperBound<TNumber>>
    where TNumber : struct, INumber<TNumber>, IMinMaxValue<TNumber>
{
    bool IsGreaterThan(TNumber number);
    TNumber Value { get; }
    /// <summary>
    /// Returns null if <see cref="Value"/> is <see cref="MinusInfinity{TNumber}.Instance"/> or <see cref="Infinity{TNumber}.Instance"/>
    /// </summary>
    TNumber? ValueOrNullIfIsInfinite { get; }
}

public static class UpperBoundMin
{
    public static IUpperBound<TNumber> Of<TNumber>(IUpperBound<TNumber> first, IUpperBound<TNumber> second)
    where TNumber : struct, INumber<TNumber>, IMinMaxValue<TNumber> =>
        first.CompareTo(second) <= 0 ?
            first :
            second;
}
public static class UpperBoundMax
{
    public static IUpperBound<TNumber> Of<TNumber>(IUpperBound<TNumber> first, IUpperBound<TNumber> second)
    where TNumber : struct, INumber<TNumber>, IMinMaxValue<TNumber> =>
        first.CompareTo(second) >= 0 ?
            first :
            second;
}


public abstract class UpperBound<TNumber> :
    ValueObject<TNumber>,
    IUpperBound<TNumber>
    where TNumber : struct, INumber<TNumber>, IMinMaxValue<TNumber>
{
    public abstract bool IsGreaterThan(TNumber number);
    public TNumber? ValueOrNullIfIsInfinite => Value;
    public virtual int CompareTo(IUpperBound<TNumber>? other) =>
        other is null ?
            CompareToResult.WhenOtherIsNull :
            Value.CompareTo(other.Value);


    public sealed class Inclusive : UpperBound<TNumber>
    {
        public override bool IsGreaterThan(TNumber number) =>
            Value >= number;

        public override int CompareTo(IUpperBound<TNumber>? other)
        {
            int result = base.CompareTo(other);
            if (result != 0)
            {
                return result;
            }

            // Compare by type when other value is equal
            return other switch
            {
                UpperBound<TNumber>.Exclusive finiteLowerBound => 1,
                UpperBound<TNumber>.Inclusive finiteLowerBound => 0,
                _ => throw TypeUndefinedException.FromCustomMessage($"Unknown upper bound type: {other!.GetType()}"),
            };
        }

        public static UpperBound<TNumber>.Inclusive From(TNumber value) => new() { Value = value, };

        public static IUpperBound<TNumber> OrInfinityFrom(TNumber? value) => value is null ?
            Infinity<TNumber>.Instance :
            new UpperBound<TNumber>.Inclusive() { Value = value.Value, };

        private Inclusive() { }
    }


    public sealed class Exclusive : UpperBound<TNumber> 
    {
        public override bool IsGreaterThan(TNumber number) =>
            Value > number;

        public override int CompareTo(IUpperBound<TNumber>? other)
        {
            int result = base.CompareTo(other);
            if (result != 0)
            {
                return result;
            }

            // Compare by type when other value is equal
            return other switch
            {
                UpperBound<TNumber>.Exclusive finiteLowerBound => 0,
                UpperBound<TNumber>.Inclusive finiteLowerBound => -1,
                _ => throw TypeUndefinedException.FromCustomMessage($"Unknown upper bound type: {other.GetType()}"),
            };
        }

        public static UpperBound<TNumber>.Exclusive From(TNumber value) => new() { Value = value, };

        public static IUpperBound<TNumber> OrInfinityFrom(TNumber? value) => value is null ?
            Infinity<TNumber>.Instance :
            new UpperBound<TNumber>.Exclusive() { Value = value.Value, };

        private Exclusive() { }
    }
}
