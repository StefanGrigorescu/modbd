using System.Numerics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace MODBD_Common.NumericTypes.Ranges;

public sealed record NullableNumberRange<TNumber>
    where TNumber : struct, INumber<TNumber>
{
    public required TNumber? LowerBound { get; init; }
    public required TNumber? UpperBound { get; init; }

    public static readonly NullableNumberRange<TNumber> Null = new() { LowerBound = null, UpperBound = null, };

    public static NullableNumberRange<TNumber> From(TNumber? lowerBound, TNumber? upperBound)
    {
        if (lowerBound is null && upperBound is null) { return Null; }
        NumberRangeException<TNumber>.ThrowIfIsLowerBoundGreaterThanUpperBound(lowerBound, upperBound);
        return new()
        {
            LowerBound = lowerBound,
            UpperBound = upperBound,
        };
    }

    /// <summary>
    /// This method checks if the <see cref="LowerBound"/> is greater than the <paramref name="otherNumber"/>. <br></br>
    /// If the <see cref="LowerBound"/> is null, it returns false. 
    /// </summary>
    /// <param name="otherNumber">The number to be compared to the <see cref="LowerBound"/>.</param>
    /// <returns>True if the <see cref="LowerBound"/> is not null and is greater than the <paramref name="otherNumber"/>, or false otherwise.</returns>
    [MemberNotNullWhen(true, nameof(LowerBound))]
    public bool IsLowerBoundGreaterThan(TNumber otherNumber) =>
        LowerBound.HasValue &&
        LowerBound.Value > otherNumber;

    /// <summary>
    /// This method checks if the <see cref="UpperBound"/> is lower than the <paramref name="otherNumber"/>. <br></br>
    /// If the <see cref="UpperBound"/> is null, it returns false. 
    /// </summary>
    /// <param name="otherNumber">The number to be compared to the <see cref="UpperBound"/>.</param>
    /// <returns>True if the <see cref="UpperBound"/> is not null and is lower than the <paramref name="otherNumber"/>, or false otherwise.</returns>
    [MemberNotNullWhen(true, nameof(UpperBound))]
    public bool IsUpperBoundLowerThan(TNumber otherNumber) =>
        UpperBound.HasValue &&
        UpperBound.Value < otherNumber;

    /// <summary>
    /// Throws if <see cref="LowerBound"/> has value and number is less than <see cref="LowerBound"/> <br></br>
    /// or <see cref="UpperBound"/> has value and number is greater than <see cref="UpperBound"/>.
    /// </summary>
    /// <param name="number"></param>
    public void ThrowIfDoesNotContain(
        TNumber number,
        [CallerArgumentExpression(nameof(number))] string parameter = "number"
    ) {
        OutOfRangeException<TNumber>.ThrowIfIsNumberGreaterThanRangeUpperBound(number, this, parameter);
        OutOfRangeException<TNumber>.ThrowIfIsNumberLowerThanRangeLowerBound(number, this, parameter);
    }

    private NullableNumberRange() { }
}
