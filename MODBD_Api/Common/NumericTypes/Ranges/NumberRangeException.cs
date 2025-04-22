using System.Numerics;
using MODBD_Api.Common.Abstractions.DomainExceptions;

namespace MODBD_Api.Common.NumericTypes.Ranges;

public sealed class NumberRangeException<TNumber> : ValueObjectException
    where TNumber : struct, INumber<TNumber>
{
    public static void ThrowIfIsLowerBoundGreaterThanUpperBound(TNumber? lowerBound, TNumber? upperBound)
    {
        if (lowerBound is not null &&
            upperBound is not null &&
            lowerBound > upperBound)
        {
            throw _lowerBoundGreaterThanUpperBound;
        }
    }

    private static readonly NumberRangeException<TNumber> _lowerBoundGreaterThanUpperBound = new(
        $"The {nameof(NullableNumberRange<TNumber>.LowerBound)} must be less or equal to {nameof(NullableNumberRange<TNumber>.UpperBound)}!");

    private NumberRangeException(string message) : base(message) { }
}
