using System.Numerics;

namespace MODBD_Common.NumericTypes.Ranges;

public interface INumberSet<TNumber>
    where TNumber : struct, INumber<TNumber>, IMinMaxValue<TNumber>
{ }


public sealed record NullNumberSet<TNumber> : INumberSet<TNumber>
    where TNumber : struct, INumber<TNumber>, IMinMaxValue<TNumber>
{
    public static readonly NullNumberSet<TNumber> Instance = new();
    private NullNumberSet() { }
}
