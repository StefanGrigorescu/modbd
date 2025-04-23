using System.Numerics;
using MODBD_Common.Abstractions.DomainExceptions;

namespace MODBD_Common.NumericTypes.Positive;

public class NotStrictlyPositiveException<TNumber> : ValueObjectException
    where TNumber : struct, INumber<TNumber>
{
    public static void ThrowIfIsNotStrictlyPositive(TNumber value)
    {
        if (value <= TNumber.Zero)
        {
            throw new NotStrictlyPositiveException<TNumber>(value);
        }
    }

    private NotStrictlyPositiveException(TNumber value) :
        base($"Value '{value}' must be greater than 0.") { }
}
