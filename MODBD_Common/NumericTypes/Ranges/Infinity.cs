using MODBD_Common.Abstractions.DiscriminatedUnions;
using MODBD_Common.NullableTypes;
using System.Numerics;

namespace MODBD_Common.NumericTypes.Ranges;

public sealed record MinusInfinity<TNumber> :
    ILowerBound<TNumber>,
    IUpperBound<TNumber>
    where TNumber : struct, INumber<TNumber>, IMinMaxValue<TNumber>
{
    public bool IsLowerThan(TNumber number) => true;
    public bool IsGreaterThan(TNumber number) => false;

    public TNumber Value =>  TNumber.MinValue;
    public TNumber? ValueOrNullIfIsInfinite => null;

    public int CompareTo(ILowerBound<TNumber>? other)
    {
        if (other is null)
        {
            return CompareToResult.WhenOtherIsNull;
        }

        int result = Value.CompareTo(other.Value);
        if (result != 0)
        {
            return result;
        }

        // Compare by type when other value is equal
        return other switch
        {
            MinusInfinity<TNumber> => 0,
            LowerBound<TNumber> finiteLowerBound => -1, // Finite Lower bound with value set to min is greater than MinusInfinity
            _ => throw TypeUndefinedException.FromCustomMessage($"Unknown lower bound type: {other.GetType()}"),
        };
    }

    public int CompareTo(IUpperBound<TNumber>? other)
    {
        if (other is null)
        {
            return CompareToResult.WhenOtherIsNull;
        }

        int result = Value.CompareTo(other.Value);
        if (result != 0)
        {
            return result;
        }

        // Compare by type when other value is equal
        return other switch
        {
            MinusInfinity<TNumber> => 0,
            UpperBound<TNumber> finiteLowerBound => -1, // Finite Lower bound with value set to min is greater than MinusInfinity
            _ => throw TypeUndefinedException.FromCustomMessage($"Unknown lower bound type: {other.GetType()}"),
        };
    }

    public override string ToString() => 
        $"-Infinity({typeof(TNumber)})";

    public static readonly MinusInfinity<TNumber> Instance = new();
    private MinusInfinity() { }
}


public sealed record Infinity<TNumber> :
    ILowerBound<TNumber>,
    IUpperBound<TNumber>
    where TNumber : struct, INumber<TNumber>, IMinMaxValue<TNumber>
{
    public bool IsLowerThan(TNumber number) => false;
    public bool IsGreaterThan(TNumber number) => true;

    public TNumber Value => TNumber.MaxValue;
    public TNumber? ValueOrNullIfIsInfinite => null;

    public int CompareTo(ILowerBound<TNumber>? other)
    {
        if (other is null)
        {
            return CompareToResult.WhenOtherIsNull;
        }
        
        int result = Value.CompareTo(other.Value);
        if(result != 0)
        {
            return result;
        }

        // Compare by type when other value is equal
        return other switch
        {
            LowerBound<TNumber> finiteLowerBound => 1,  // Finite Lower bound with value set to max is less than Infinity
            Infinity<TNumber> => 0,
            _ => throw TypeUndefinedException.FromCustomMessage($"Unknown lower bound type: {other.GetType()}"),
        };
    }

    public int CompareTo(IUpperBound<TNumber>? other)
    {
        if (other is null)
        {
            return CompareToResult.WhenOtherIsNull;
        }

        int result = Value.CompareTo(other.Value);
        if (result != 0)
        {
            return result;
        }

        // Compare by type when other value is equal
        return other switch
        {
            UpperBound<TNumber> finiteLowerBound => 1,  // Finite Lower bound with value set to max is less than Infinity
            Infinity<TNumber> => 0,
            _ => throw TypeUndefinedException.FromCustomMessage($"Unknown upper bound type: {other.GetType()}"),
        };
    }

    public override string ToString() =>
        $"Infinity({typeof(TNumber)})";

    public static readonly Infinity<TNumber> Instance = new();
    private Infinity() { }
}
