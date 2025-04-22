using System.Numerics;
using System.Runtime.CompilerServices;

namespace MODBD_Api.Common.NumericTypes.Ranges;

public sealed record NumberRange<TNumber>
    where TNumber : struct, INumber<TNumber>
{
    public required TNumber LowerBound { get; init; }
    public required TNumber UpperBound { get; init; }

    public static NumberRange<TNumber> From(TNumber lowerBound, TNumber upperBound)
    {
        NumberRangeException<TNumber>.ThrowIfIsLowerBoundGreaterThanUpperBound(lowerBound, upperBound);
        return new()
        {
            LowerBound = lowerBound,
            UpperBound = upperBound,
        };
    }

    /// <summary>
    /// Throws if number is less than <see cref="LowerBound"/> or greater than <see cref="UpperBound"/>.
    /// </summary>
    /// <param name="number"></param>
    public void ThrowIfDoesNotContain(
        TNumber number, 
        [CallerArgumentExpression(nameof(number))] string parameter = "number"
    ) {
        OutOfRangeException<TNumber>.ThrowIfIsNumberGreaterThanRangeUpperBound(number, this, parameter);
        OutOfRangeException<TNumber>.ThrowIfIsNumberLowerThanRangeLowerBound(number, this, parameter);
    }

    private NumberRange() { }
}
