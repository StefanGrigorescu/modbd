using MODBD_Common.Abstractions;
using System.Numerics;

namespace MODBD_Core.Applications;

public sealed class SqlValue<T> : ValueObject<T>
    where T : struct, IComparable<T>, IEquatable<T>, INumber<T>, IMinMaxValue<T>
{
    public static SqlValue<T> From(T value) => new() { Value = value, };
    private SqlValue() { }
}
