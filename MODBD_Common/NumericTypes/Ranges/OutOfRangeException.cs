using System.Numerics;
using System.Runtime.CompilerServices;
using MODBD_Common.Abstractions.DomainExceptions;

namespace MODBD_Common.NumericTypes.Ranges;

public sealed class OutOfRangeException<TNumber> : ValueObjectException
    where TNumber : struct, INumber<TNumber>
{
    public static void ThrowIfIsNumberLowerThanRangeLowerBound(
        TNumber number,
        NumberRange<TNumber> range,
        [CallerArgumentExpression(nameof(number))] string? paramName = "'number'"
    ) {
        if (number < range.LowerBound)
        {
            throw LessThanLowerBound(number, range.LowerBound, paramName);
        }
    }

    public static void ThrowIfIsNumberGreaterThanRangeUpperBound(
        TNumber number,
        NumberRange<TNumber> range,
        [CallerArgumentExpression(nameof(number))] string? paramName = "'number'"
    ) {
        if (number > range.UpperBound)
        {
            throw GreaterThanUpperBound(number, range.UpperBound, paramName);
        }
    }

    public static void ThrowIfIsNumberLowerThanRangeLowerBound(
        TNumber number,
        NullableNumberRange<TNumber> range,
        [CallerArgumentExpression(nameof(number))] string? paramName = "'number'"
    ) {
        if (range.LowerBound.HasValue &&
            number < range.LowerBound
        ) {
            throw LessThanLowerBound(number, range.LowerBound.Value, paramName);
        }
    }

    public static void ThrowIfIsNumberGreaterThanRangeUpperBound(
        TNumber number,
        NullableNumberRange<TNumber> range,
        [CallerArgumentExpression(nameof(number))] string? paramName = "'number'"
    ) {
        if (range.UpperBound.HasValue &&
            number > range.UpperBound
        ) {
            throw GreaterThanUpperBound(number, range.UpperBound.Value, paramName);
        }
    }

    public static OutOfRangeException<TNumber> LessThanLowerBound(
        TNumber actualValue,
        TNumber lowerBound,
        [CallerArgumentExpression(nameof(actualValue))] string? paramName = "'actualValue'"
    ) => new(
        $"{paramName} value must be greater or equal to {lowerBound}! Actual value is {actualValue}.");

    public static OutOfRangeException<TNumber> GreaterThanUpperBound(
        TNumber actualValue,
        TNumber upperBound,
        [CallerArgumentExpression(nameof(actualValue))] string? paramName = "'actualValue'"
    ) => new(
        $"{paramName} value must be lower or equal to {upperBound}! Actual value is {actualValue}.");

    private OutOfRangeException(string message) : base(message) { }
}
