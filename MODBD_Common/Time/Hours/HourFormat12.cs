using System.Diagnostics;
using MODBD_Common.NullableTypes;
using MODBD_Common.NumericTypes.Positive;
using MODBD_Common.NumericTypes.Ranges;

namespace MODBD_Common.Time.Hours;

/// <summary>
/// 12-hour format <br></br>
/// This format represents the hours of the day from 12:00 AM (midnight) to 11:59 PM (just before the next midnight). <br></br>
/// It uses "AM" (ante meridiem) and "PM" (post meridiem) to distinguish between morning and afternoon/evening times. 
/// 
/// <para>
/// For example, 2:00 PM represents 14:00 in the 24-hour format.
/// </para>
/// 
/// <para>
/// In the 12-hour format, the hours progress as follows:
///     12:00 AM(midnight)
///     1:00 AM
///     2:00 AM
///     ...
///     11:00 AM
///     12:00 PM(noon)
///     1:00 PM
///     2:00 PM
///     ...
///     11:00 PM
/// There is no 0 hour in the 12-hour format.The day starts at 12:00 AM (midnight) and progresses to 11:59 PM.
/// </para>
/// </summary>
[DebuggerDisplay("{Value}:00 {DayPeriod}")]
public sealed record HourFormat12 : IHour, IComparable<HourFormat12>
{
    public required int Value { get; init; }
    public required DayPeriod DayPeriod { get; init; }

    public (Positive<int> DaySpan, IHour Hour) AddHours(Positive<int> hours)
    {
        (Positive<int> DaySpan, IHour Hour) = HourFormat24.From(this)
            .AddHours(hours);
        return (
            DaySpan: DaySpan,
            Hour: From(Hour)
        );
    }

    public (Positive<int> DaySpan, IHour Hour) SubstractHours(Positive<int> hours)
    {
        (Positive<int> DaySpan, IHour Hour) = HourFormat24.From(this)
            .SubstractHours(hours);
        return (
            DaySpan: DaySpan,
            Hour: From(Hour)
        );
    }

    public int CompareTo(IHour? other) =>
        other is null ?
            CompareToResult.WhenOtherIsNull :
            CompareTo(From(other));

    public int CompareTo(HourFormat12? other)
    {
        if (other is null)
        {
            return CompareToResult.WhenOtherIsNull;
        }

        // Compare DayPeriod first (AM < PM)
        int dayPeriodComparison = DayPeriod.CompareTo(other.DayPeriod);
        if (dayPeriodComparison != 0)
        {
            return dayPeriodComparison;
        }

        if (Value == 12 && other.Value != 12)
        {
            return -1; // 12 AM/PM should come before 1-11 AM/PM
        }
        if (Value != 12 && other.Value == 12)
        {
            return 1; // 1-11 AM/PM should come after 12 AM/PM
        }

        return Value.CompareTo(other.Value);
    }

    public TimeOnly ToTimeOnly() => new(
        hour: HourFormat24.From(this).Value, 
        minute: 0
    );

    public int ToInt() =>
        HourFormat24.From(this).ToInt();

    public string ToShortString() => 
        HourFormat24.From(this).ToShortString();

    private static readonly NumberRange<int> _range = NumberRange<int>.FromFiniteInclusive(1, 12);

    public static HourFormat12 From(IHour hour) => hour switch
    {
        HourFormat12 hour12 => hour12,
        HourFormat24 hour24 => From(hour24),
        _ => throw new NotSupportedException($"Could not construct {nameof(HourFormat12)} instance. Type {hour.GetType().Name} was unexpected"),
    };

    public static HourFormat12 From(HourFormat24 hour24)
    {
        int hourValue = hour24.Value;

        bool isAM = hourValue >= 0 && hourValue < 12;
        if (isAM)
        {
            if (hourValue == 0)
            {
                hourValue = 12; // Convert 0 to 12 AM (midnight)
            }
            return new()
            {
                Value = hourValue,
                DayPeriod = DayPeriod.AM,
            };
        }

        if (hourValue > 12)
        {
            hourValue -= 12; // Convert 13-23 to 1-11 PM
        }
        return new HourFormat12
        {
            Value = hourValue,
            DayPeriod = DayPeriod.PM,
        };
    }

    public static HourFormat12 From(int hour, DayPeriod dayPeriod)
    {
        _range.ThrowIfDoesNotContain(hour);
        return new()
        {
            Value = hour,
            DayPeriod = dayPeriod,
        };
    }

    private HourFormat12() { }
}
