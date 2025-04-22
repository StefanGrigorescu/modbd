using System.Diagnostics.CodeAnalysis;
using MODBD_Api.Common.Abstractions.DomainExceptions;

namespace MODBD_Api.Common.Time.Hours;

public sealed class TimeUnitsTooSmallSpecifiedException : ValueObjectException
{
    public static DateTime? ThrowIfHasTimeUnitsSmallerThanHourSpecified(
        [NotNullIfNotNull(nameof(time))] DateTime? time
    )
    {
        return time is null ?
            null :
            ThrowIfHasTimeUnitsSmallerThanHourSpecified(time.Value);
    }

    public static DateTime ThrowIfHasTimeUnitsSmallerThanHourSpecified(DateTime time)
    {
        if (
            time.Minute != 0 ||
            time.Second != 0 ||
            time.Millisecond != 0)
        {
            throw new TimeUnitsTooSmallSpecifiedException();
        }

        return time;
    }

    private TimeUnitsTooSmallSpecifiedException() :
        base($"Time units smaller than hour must be zero for this {nameof(DateTime)} instance.")
    { }
}


public static class ThrowIfHasTimeUnitsTooSmallSpecifiedExtensions
{
    public static DateTime? ThrowIfHasTimeUnitsSmallerThanHourSpecified(
        [NotNullIfNotNull(nameof(time))] this DateTime? time
    ) => TimeUnitsTooSmallSpecifiedException.ThrowIfHasTimeUnitsSmallerThanHourSpecified(time);

    public static DateTime ThrowIfHasTimeUnitsSmallerThanHourSpecified(this DateTime time) =>
        TimeUnitsTooSmallSpecifiedException.ThrowIfHasTimeUnitsSmallerThanHourSpecified(time);
}
