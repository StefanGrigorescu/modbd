namespace MODBD_Api.Common.Time.DateTimes;

public static class DateTimeExtensions
{
    public static bool IsEarlierThan(this DateTime thisDateTime, DateTime other) =>
        thisDateTime.CompareTo(other) < 0;

    public static bool IsLaterThan(this DateTime thisDateTime, DateTime other) =>
        thisDateTime.CompareTo(other) < 0;

    /// <summary>
    /// Determines whether two <see cref="DateTime"/> instances are equal, considering no allowed maximum error.
    /// </summary>
    /// <param name="thisDateTime">The first <see cref="DateTime"/> instance.</param>
    /// <param name="other">The second <see cref="DateTime"/> instance.</param>
    /// <returns>
    /// <see langword="true"/> if the two <see cref="DateTime"/> instances are equal; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool IsEqual(this DateTime thisDateTime, DateTime other) =>
        thisDateTime.IsEqual(other, 0);

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
    public static bool IsEqual(this DateTime thisDateTime, DateTime other, int allowedMaxErrorInMs)
    {
        // Ensure both DateTime values are in UTC
        if (thisDateTime.Kind != DateTimeKind.Utc)
        {
            thisDateTime = thisDateTime.ToUniversalTime();
        }
        if (other.Kind != DateTimeKind.Utc)
        {
            other = other.ToUniversalTime();
        }

        // Compare with the allowed maximum error in milliseconds
        double differenceInMs = Math.Abs((thisDateTime - other).TotalMilliseconds);
        return differenceInMs <= allowedMaxErrorInMs;
    }
}
