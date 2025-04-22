using MODBD_Api.Common.Abstractions;
using MODBD_Api.Common.NullableTypes;
using MODBD_Api.Common.Time.DayHourSpans;

namespace MODBD_Api.Common.Time.DateTimes;

public sealed class FullDateTime : ValueObject<DateTime>, IDateTime<FullDateTime>
{
    public bool IsEarlierThan(FullDateTime other) =>
        CompareTo(other) < 0;

    public bool IsLaterThan(FullDateTime other) =>
        CompareTo(other) < 0;

    /// <summary>
    /// Determines whether two <see cref="DateTime"/> instances are equal, considering no allowed maximum error.
    /// </summary>
    /// <param name="thisDateTime">The first <see cref="DateTime"/> instance.</param>
    /// <param name="other">The second <see cref="DateTime"/> instance.</param>
    /// <returns>
    /// <see langword="true"/> if the two <see cref="DateTime"/> instances are equal; otherwise, <see langword="false"/>.
    /// </returns>
    public bool IsEqual(FullDateTime other) =>
        IsEqual(other, 0);

    /// <summary>
    /// Determines whether two <see cref="DateTime"/> instances are equal, considering an allowed minimum error in milliseconds.
    /// </summary>
    /// <param name="thisDateTime">The first <see cref="DateTime"/> instance.</param>
    /// <param name="other">The second <see cref="DateTime"/> instance.</param>
    /// <param name="allowedMaxErrorInMs">The allowed maximum error in milliseconds. <br></br>
    /// <see cref="DateTime"/>s with absolute differences below this value will be considered equal.</param>
    /// <returns>
    /// <see langword="true"/> if the absolute difference between the two <see cref="DateTime"/> instances is less than or equal to the allowed minimum error; otherwise, <see langword="false"/>.
    /// </returns>
    public bool IsEqual(FullDateTime other, int allowedMaxErrorInMs)
    {
        DateTime thisValue = Value;
        DateTime otherValue = other.Value;

        // Ensure both DateTime values are in UTC
        if (thisValue.Kind != DateTimeKind.Utc)
        {
            thisValue = thisValue.ToUniversalTime();
        }
        if (otherValue.Kind != DateTimeKind.Utc)
        {
            otherValue = otherValue.ToUniversalTime();
        }

        // Compare with the allowed maximum error in milliseconds
        double differenceInMs = Math.Abs((thisValue - otherValue).TotalMilliseconds);
        return differenceInMs <= allowedMaxErrorInMs;
    }

    /// <summary>
    /// Returns a new <see cref="DateTime"/> that adds the specified number of days and hours to the value of this instance.
    /// </summary>
    /// <param name="modifier">A number of whole days and hours. The parameter can be only positive.</param>
    /// <returns>
    /// An object whose value is the sum of the date and time represented by this instance and the number of days and hours represented by <paramref name="modifier"/>.
    /// </returns>
    public FullDateTime Add(DayHourSpan modifier) => new()
    {
        Value = Value
            .AddDays(modifier.Days)
            .AddHours(modifier.Hours),
    };

    /// <summary>
    /// Returns a new <see cref="DateTime"/> that substracts the specified number of days and hours to the value of this instance.
    /// </summary>
    /// <param name="modifier">A number of whole days and hours. The parameter can be only positive.</param>
    /// <returns>
    /// An object whose value is the difference between the date and time represented by this instance and the number of days and hours represented by <paramref name="modifier"/>.
    /// </returns>
    public FullDateTime Substract(DayHourSpan modifier) => new()
    {
        Value = Value
            .AddDays(-1 * modifier.Days)
            .AddHours(-1 * modifier.Hours),
    };

    public TimeSpan Substract(FullDateTime other) => this - other;
    public static TimeSpan operator -(FullDateTime thisDateHour, FullDateTime other)
    {
        thisDateHour.ValidateNotNull();
        other.ValidateNotNull();

        return thisDateHour.Value - other.Value;
    }

    public DateTime ToDateTime() => Value;

    public static FullDateTime From(DateTime value) => new() { Value = value, };

    private FullDateTime() { }
}
