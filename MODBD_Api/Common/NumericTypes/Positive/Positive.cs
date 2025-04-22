using System.Numerics;
using MODBD_Api.Common.Abstractions;

namespace MODBD_Api.Common.NumericTypes.Positive;

public sealed class Positive<TNumber> : ValueObject<TNumber>
    where TNumber : struct, INumber<TNumber>
{
    public static readonly Positive<TNumber> Zero = new() { Value = TNumber.Zero, };
    public static readonly Positive<TNumber> One = new() { Value = TNumber.One, };

    public static Positive<TNumber> From(TNumber value)
    {
        NegativeException<TNumber>.ThrowIfIsNegative(value);
        return new() { Value = value, };
    }
    public static Positive<TNumber> FromDbStoredValue(TNumber value) => new() { Value = value, };

    public Positive<TNumber> Plus(TNumber value) => new() { Value = Value + value, };
    public Positive<TNumber> Multiply(TNumber value)
    {
        NegativeException<TNumber>.ThrowIfIsNegative(value);
        return new() { Value = Value * value, };
    }

    private Positive() : base() { }
}
