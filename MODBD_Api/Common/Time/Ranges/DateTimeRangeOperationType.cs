using MODBD_Api.Common.Abstractions.DiscriminatedUnions;
using MODBD_Api.Common.Time.DateTimes;
using MODBD_Api.Common.Time.DayHourSpans;

namespace MODBD_Api.Common.Time.Ranges;

public sealed class DateTimeRangeOperationType<TDateTime> : Enumeration<DateTimeRangeOperationType<TDateTime>>
    where TDateTime : class, IDateTime<TDateTime>, IComparable<TDateTime>
{
    public required Func<DateTimeRange<TDateTime>, DayHourSpan, DateTimeRange<TDateTime>> Apply { get; init; }

    public static readonly DateTimeRangeOperationType<TDateTime> ShiftPeriod = new()
    {
        Id = 0,
        Name = "Shift",
        Apply = (dateTimeRange, modifier) =>
            dateTimeRange.Shift(modifier),
    };

    public static readonly DateTimeRangeOperationType<TDateTime> ExtendPeriod = new()
    {
        Id = 1,
        Name = "Extend",
        Apply = (dateTimeRange, modifier) =>
            dateTimeRange.Extend(modifier),
    };

    public static readonly DateTimeRangeOperationType<TDateTime> StartEarlier = new()
    {
        Id = 2,
        Name = "Start earlier",
        Apply = (dateTimeRange, modifier) =>
            dateTimeRange.StartEarlier(modifier),
    };

    public static readonly DateTimeRangeOperationType<TDateTime> EndEarlier = new()
    {
        Id = 3,
        Name = "End earlier",
        Apply = (dateTimeRange, modifier) =>
            dateTimeRange.EndEarlier(modifier),
    };
}
