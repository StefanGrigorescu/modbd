using MODBD_Common.NullableTypes;
using MODBD_Common.NumericTypes.Positive;
using MODBD_Common.Text;
using MODBD_Common.Time.DayHourSpans;
using MODBD_Common.Time.Hours;

namespace MODBD_Common.Time.DateTimes;

public sealed record DateHour : IDateTime<DateHour>, IComparable<DateHour>
{
    public required DateOnly Date { get; init; }
    public required IHour Hour { get; init; }

    public int CompareTo(DateHour? other)
    {
        if (other is null)
        {
            return CompareToResult.WhenThisIsNull;
        }

        int dateComparison = Date.CompareTo(other.Date);
        if (dateComparison != 0)
        {
            return dateComparison;
        }

        return Hour.CompareTo(other.Hour);
    }

    /// <summary>
    /// Returns a new <see cref="DateHour"/> that adds the specified number of days and hours to the value of this instance.
    /// </summary>
    /// <param name="modifier">A number of whole days and hours. The parameter can be only positive.</param>
    /// <returns>
    /// An object whose value is the sum of the date and time represented by this instance and the number of days and hours represented by <paramref name="modifier"/>.
    /// </returns>
    public DateHour Add(DayHourSpan modifier)
    {
        return 
            AddDays(modifier.Days)
            .AddHours(modifier.Hours);
    }

    /// <summary>
    /// Returns a new <see cref="DateHour"/> that substracts the specified number of days and hours to the value of this instance.
    /// </summary>
    /// <param name="modifier">A number of whole days and hours. The parameter can be only positive.</param>
    /// <returns>
    /// An object whose value is the difference between the date and time represented by this instance and the number of days and hours represented by <paramref name="modifier"/>.
    /// </returns>
    public DateHour Substract(DayHourSpan modifier)
    {
        return
            SubstractDays(modifier.Days)
            .SubstractHours(modifier.Hours);
    }

    public DateHour AddDays(Positive<int> days) => new()
    {
        Date = Date.AddDays(days),
        Hour = Hour,
    };

    public DateHour SubstractDays(Positive<int> hours) => new()
    {
        Date = Date.AddDays(-1 * hours),
        Hour = Hour,
    };

    public DateHour AddHours(Positive<int> hours)
    {
        (Positive<int> DaySpan, IHour Hour) result = Hour.AddHours(hours);
        return new DateHour
        {
            Date = Date.AddDays(result.DaySpan),
            Hour = result.Hour,
        };
    }

    public DateHour SubstractHours(Positive<int> hours)
    {
        (Positive<int> DaySpan, IHour Hour) result = Hour.SubstractHours(hours);
        return new DateHour
        {
            Date = Date.AddDays(-1 * result.DaySpan),
            Hour = result.Hour,
        };
    }

    public TimeSpan Substract(DateHour other) => this - other;
    public static TimeSpan operator -(DateHour thisDateHour, DateHour other)
    {
        thisDateHour.ValidateNotNull();
        other.ValidateNotNull();

        return thisDateHour.ToDateTime() - other.ToDateTime();
    }

    public DateTime ToDateTime() =>
        Date.ToDateTime(Hour.ToTimeOnly());

    public int ToInt() =>
        int.Parse(ToShortString());

    public string ToShortString() => 
        $"{Date.Year}{Date.Month:00}{Date.Day:00}{Hour.ToShortString()}";

    public static int ShortStringLength => 10;

    public static DateHour From(DateTime? date)
    {
        DateTime dateTime = date.ValidateNotNull();
        // Trim all time units smaller than minutes.
        // Then, if minutes are still present, throw an exception. 
        int hour = TrimTimeUnitsSmallerThan.Hour.Normalize(dateTime)
            .ThrowIfHasTimeUnitsSmallerThanHourSpecified()
            .Hour;
        return new()
        {
            Date = DateOnly.FromDateTime(dateTime),
            Hour = HourFormat24.From(hour),
        };
    }

    public static DateHour From(DateOnly date, IHour hour) => new()
    {
        Date = date,
        Hour = hour,
    };

    public static DateHour FromInt(int dateHourInt)
    {
        int year = dateHourInt / 1000000;
        int month = (dateHourInt / 10000) % 100;
        int day = (dateHourInt / 100) % 100;
        int hour = dateHourInt % 100;

        return new()
        {
            Date = new DateOnly(year, month, day),
            Hour = HourFormat24.From(hour),
        };
    }

    public static DateHour FromShortString(string dateHourShortString)
    {
        dateHourShortString.ValidateNotNullOrEmpty();
        TextHasNotExpectedLengthException.ThrowIfHasNotExpectedLength(dateHourShortString, ShortStringLength);

        ReadOnlySpan<char> span = dateHourShortString.AsSpan();

        int year = int.Parse(span.Slice(0, 4));
        int month = int.Parse(span.Slice(4, 2));
        int day = int.Parse(span.Slice(6, 2));
        int hour = int.Parse(span.Slice(8, 2));

        return new DateHour
        {
            Date = new DateOnly(year, month, day),
            Hour = HourFormat24.From(hour),
        };
    }

    private DateHour() {  }
}


public static class DateHourFormat
{
    public const string yyyyMMddHH = "yyyyMMddHH";
}
