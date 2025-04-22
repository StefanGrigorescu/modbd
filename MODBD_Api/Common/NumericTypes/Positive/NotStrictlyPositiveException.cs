using System.Numerics;
using MODBD_Api.Common.Abstractions.DomainExceptions;

namespace MODBD_Api.Common.NumericTypes.Positive;

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
