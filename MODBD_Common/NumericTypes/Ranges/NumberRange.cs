using System.Numerics;
using System.Runtime.CompilerServices;
using MODBD_Common.Abstractions;

namespace MODBD_Common.NumericTypes.Ranges;

public sealed record NumberRange<TNumber> : 
    INumberSet<TNumber>,
    IImmutable
    where TNumber : struct, INumber<TNumber>, IMinMaxValue<TNumber>
{
    public required ILowerBound<TNumber> LowerBound { get; init; }
    public required IUpperBound<TNumber> UpperBound { get; init; }

    public INumberSet<TNumber> Intersect(NumberRange<TNumber> other)
    {
        ILowerBound<TNumber> greatestLowerBound = LowerBoundMax.Of(LowerBound, other.LowerBound);
        IUpperBound<TNumber> leastUpperBound = UpperBoundMin.Of(UpperBound, other.UpperBound);

        return NumberRange<TNumber>.OrNullNumberSetFrom(greatestLowerBound, leastUpperBound);
    }

    public NumberRangesReunion<TNumber> AsReunion() => NumberRangesReunion<TNumber>.From([this]);

    public static readonly NumberRange<TNumber> Unbound = new() { LowerBound = MinusInfinity<TNumber>.Instance, UpperBound = Infinity<TNumber>.Instance, };

    public static NumberRange<TNumber> FromFiniteInclusive(TNumber lowerBound, TNumber upperBound) => From(
            LowerBound<TNumber>.Inclusive.From(lowerBound),
            UpperBound<TNumber>.Inclusive.From(upperBound)
    );

    public static NumberRange<TNumber> FromFiniteExclusive(TNumber lowerBound, TNumber upperBound) => From(
        LowerBound<TNumber>.Exclusive.From(lowerBound),
        UpperBound<TNumber>.Exclusive.From(upperBound)
    );

    public static NumberRange<TNumber> From(ILowerBound<TNumber> lowerBound, IUpperBound<TNumber> upperBound)
    {
        if (lowerBound is MinusInfinity<TNumber> && upperBound is Infinity<TNumber>) { return Unbound; }
        NumberRangeException<TNumber>.ThrowIfIsLowerBoundGreaterThanUpperBound(lowerBound, upperBound);
        return new()
        {
            LowerBound = lowerBound,
            UpperBound = upperBound,
        };
    }

    public static INumberSet<TNumber> OrNullNumberSetFrom(ILowerBound<TNumber> lowerBound, IUpperBound<TNumber> upperBound)
    {
        if (lowerBound is MinusInfinity<TNumber> && upperBound is Infinity<TNumber>) { return Unbound; }
        return BoundsComparer.IsLowerBoundGreaterThanUpperBound(lowerBound, upperBound) ?
            NullNumberSet<TNumber>.Instance :
            new NumberRange<TNumber>()
            {
                LowerBound = lowerBound,
                UpperBound = upperBound,
            };
    }

    public bool Contains(TNumber number) =>
        LowerBound.IsLowerThan(number) &&
        UpperBound.IsGreaterThan(number);

    /// <summary>
    /// Throws if <see cref="LowerBound"/> has value and number is less than <see cref="LowerBound"/> <br></br>
    /// or <see cref="UpperBound"/> has value and number is greater than <see cref="UpperBound"/>.
    /// </summary>
    /// <param name="number"></param>
    public void ThrowIfDoesNotContain(
        TNumber number,
        [CallerArgumentExpression(nameof(number))] string parameter = "number"
    )
    {
        if (!UpperBound.IsGreaterThan(number))
        {
            throw new ArgumentOutOfRangeException($"{parameter} value does not satisfy upper bound {UpperBound}! Actual value is {number}.");
        }
        if (!LowerBound.IsLowerThan(number))
        {
            throw new ArgumentOutOfRangeException($"{parameter} value does not satisfy lower bound {LowerBound}! Actual value is {number}.");
        }
    }

    private NumberRange() { }
}
