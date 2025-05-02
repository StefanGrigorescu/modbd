using MODBD_Common.Abstractions;
using MODBD_Common.Abstractions.DiscriminatedUnions;
using MODBD_Common.NullableTypes;
using System.Numerics;

namespace MODBD_Common.NumericTypes.Ranges;

public interface ILowerBound<TNumber> : IComparable<ILowerBound<TNumber>>
    where TNumber : struct, INumber<TNumber>, IMinMaxValue<TNumber>
{
    bool IsLowerThan(TNumber number);
    TNumber Value { get; }
    /// <summary>
    /// Returns null if <see cref="Value"/> is <see cref="MinusInfinity{TNumber}.Instance"/> or <see cref="Infinity{TNumber}.Instance"/>
    /// </summary>
    TNumber? ValueOrNullIfIsInfinite { get; }
}

public static class LowerBoundMin
{
    public static ILowerBound<TNumber> Of<TNumber>(ILowerBound<TNumber> first, ILowerBound<TNumber> second)
    where TNumber : struct, INumber<TNumber>, IMinMaxValue<TNumber> =>
        first.CompareTo(second) <= 0 ?
            first :
            second;
}
public static class LowerBoundMax
{
    public static ILowerBound<TNumber> Of<TNumber>(ILowerBound<TNumber> first, ILowerBound<TNumber> second)
    where TNumber : struct, INumber<TNumber>, IMinMaxValue<TNumber> =>
        first.CompareTo(second) >= 0 ?
            first :
            second;
}


public abstract class LowerBound<TNumber> : 
    ValueObject<TNumber>, 
    ILowerBound<TNumber>
    where TNumber : struct, INumber<TNumber>, IMinMaxValue<TNumber>
{
    public abstract bool IsLowerThan(TNumber number);
    public TNumber? ValueOrNullIfIsInfinite => Value;
    public virtual int CompareTo(ILowerBound<TNumber>? other) => 
        other is null ?
            CompareToResult.WhenOtherIsNull :
            Value.CompareTo(other.Value);


    public sealed class Inclusive : LowerBound<TNumber>
    {
        public override bool IsLowerThan(TNumber number) =>
            Value <= number;

        public override int CompareTo(ILowerBound<TNumber>? other)
        {
            int result = base.CompareTo(other);
            if (result != 0)
            {
                return result;
            }

            // Compare by type when other value is equal
            return other switch
            {
                LowerBound<TNumber>.Exclusive finiteLowerBound => -1,
                LowerBound<TNumber>.Inclusive finiteLowerBound => 0,
                _ => throw TypeUndefinedException.FromCustomMessage($"Unknown lower bound type: {other.GetType()}"),
            };
        }

        public static LowerBound<TNumber>.Inclusive From(TNumber value) => new() { Value = value, };

        public static ILowerBound<TNumber> OrMinusInfinityFrom(TNumber? value) => value is null ? 
            MinusInfinity<TNumber>.Instance : 
            new LowerBound<TNumber>.Inclusive() { Value = value.Value, };

        private Inclusive() { }
    }


    public sealed class Exclusive : LowerBound<TNumber>
    {
        public override bool IsLowerThan(TNumber number) =>
            Value < number;

        public override int CompareTo(ILowerBound<TNumber>? other)
        {
            int result = base.CompareTo(other);
            if (result != 0)
            {
                return result;
            }

            // Compare by type when other value is equal
            return other switch
            {
                LowerBound<TNumber>.Exclusive finiteLowerBound => 0,
                LowerBound<TNumber>.Inclusive finiteLowerBound => 1,
                _ => throw TypeUndefinedException.FromCustomMessage($"Unknown lower bound type: {other.GetType()}"),
            };
        }

        public static LowerBound<TNumber>.Exclusive From(TNumber value) => new() { Value = value, };

        public static ILowerBound<TNumber> OrMinusInfinityFrom(TNumber? value) => value is null ?
            MinusInfinity<TNumber>.Instance :
            new LowerBound<TNumber>.Exclusive() { Value = value.Value, };

        private Exclusive() { }
    }
}
