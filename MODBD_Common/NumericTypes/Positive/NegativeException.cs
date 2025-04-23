using MODBD_Common.Abstractions.DomainExceptions;
using System.Numerics;

namespace MODBD_Common.NumericTypes.Positive;

public sealed class NegativeException<TNumber> : ValueObjectException
    where TNumber : struct, INumber<TNumber>
{
    public static void ThrowIfIsNegative(TNumber value)
    {
        if (value < TNumber.Zero)
        {
            throw new NegativeException<TNumber>(value);
        }
    }

    private NegativeException(TNumber value) :
        base($"Negative value '{value}' is not allowed.") { }
}
