using System.Numerics;
using MODBD_Common.Abstractions;

namespace MODBD_Common.NumericTypes.Positive;

public class StrictlyPositive<TNumber> : ValueObject<TNumber>
    where TNumber : struct, INumber<TNumber>
{
    public static readonly StrictlyPositive<TNumber> One = new() { Value = TNumber.One, };

    public static StrictlyPositive<TNumber> From(TNumber value)
    {
        NotStrictlyPositiveException<TNumber>.ThrowIfIsNotStrictlyPositive(value);
        return new() { Value = value, };
    }
    public static StrictlyPositive<TNumber> FromDbStoredValue(TNumber value) => new() { Value = value, };

    public StrictlyPositive<TNumber> Plus(TNumber value) => new() { Value = Value + value, };
    public StrictlyPositive<TNumber> Multiply(TNumber value)
    {
        NotStrictlyPositiveException<TNumber>.ThrowIfIsNotStrictlyPositive(value);
        return new() { Value = Value * value, };
    }

    protected StrictlyPositive() : base() { }
}
