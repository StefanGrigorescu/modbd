using MODBD_Common.Abstractions.DomainExceptions;
using MODBD_Common.Time.DateTimes;

namespace MODBD_Common.Time.Ranges;

public sealed class DateTimeRangeException<TDateTime> : ValueObjectException
    where TDateTime : class, IDateTime<TDateTime>, IComparable<TDateTime>
{
    public static void ThrowIfIsStartAfterEnd(TDateTime? start, TDateTime? end)
    {
        if (start is not null &&
            end is not null &&
            start.CompareTo(end) > 0
        ) {
            throw new DateTimeRangeException<TDateTime>(start, end);
        }
    }

    private DateTimeRangeException(TDateTime start, TDateTime end) : base(
        $"The end date must be after the start date! Actual range is {start} - {end}."
    ) { }
}
