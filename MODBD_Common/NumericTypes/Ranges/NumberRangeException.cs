using MODBD_Common.Abstractions.DomainExceptions;
using System.Numerics;

namespace MODBD_Common.NumericTypes.Ranges;

public sealed class NumberRangeException<TNumber> : ValueObjectException
    where TNumber : struct, INumber<TNumber>, IMinMaxValue<TNumber>
{
    public static void ThrowIfIsLowerBoundGreaterThanUpperBound(ILowerBound<TNumber> lowerBound, IUpperBound<TNumber> upperBound)
    {
        if(BoundsComparer.IsLowerBoundGreaterThanUpperBound(lowerBound, upperBound))
        {
            throw new NumberRangeException<TNumber>(lowerBound, upperBound);
        }
    }

    private NumberRangeException(ILowerBound<TNumber> lowerBound, IUpperBound<TNumber> upperBound) : base(
        $"The {nameof(NumberRange<TNumber>.LowerBound)} must be less than or equal to {nameof(NumberRange<TNumber>.UpperBound)}! Actual bounds: {lowerBound} and {upperBound}."
    ) { }

    private NumberRangeException(string message) : base(message) { }
}


public static class BoundsComparer
{
    public static bool IsLowerBoundGreaterThanUpperBound<TNumber>(ILowerBound<TNumber> lowerBound, IUpperBound<TNumber> upperBound)
        where TNumber : struct, INumber<TNumber>, IMinMaxValue<TNumber> =>
        lowerBound is not MinusInfinity<TNumber> && 
        upperBound is not Infinity<TNumber> && (
            (lowerBound is LowerBound<TNumber> finiteLowerBound && !upperBound.IsGreaterThan(finiteLowerBound.Value)) ||
            (upperBound is UpperBound<TNumber> finiteUpperBound && !lowerBound.IsLowerThan(finiteUpperBound.Value))
        );
}
