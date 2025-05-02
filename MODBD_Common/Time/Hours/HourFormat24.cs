using System.Diagnostics;
using MODBD_Common.NullableTypes;
using MODBD_Common.NumericTypes.Positive;
using MODBD_Common.NumericTypes.Ranges;

namespace MODBD_Common.Time.Hours;

/// <summary>
/// 24-hour format <br></br>
/// This format represents the hours of the day from 00:00 to 23:59. <br></br>
/// It is commonly used in military, aviation, computing, and other fields where unambiguous time representation is important. 
/// <para>
/// For example, 14:00 represents 2:00 PM.
/// </para>
/// </summary>
[DebuggerDisplay("{Value}:00")]
public sealed record HourFormat24 : IHour, IComparable<HourFormat24>
{
    public required int Value { get; init; }

    public (Positive<int> DaySpan, IHour Hour) AddHours(Positive<int> hours)
    {
        int additionalDays = (Value + hours.Value) / _exclusiveUpperBound;
        int newHourValue = (Value + hours.Value) % _exclusiveUpperBound;
        return (
            DaySpan: Positive<int>.From(additionalDays), 
            Hour: From(newHourValue)
        );
    }

    public (Positive<int> DaySpan, IHour Hour) SubstractHours(Positive<int> hours)
    {
        int newHourValue = Value - hours.Value;
        if (newHourValue < 0)
        {
            newHourValue += _exclusiveUpperBound;
        }
        return (
            DaySpan: Positive<int>.From(newHourValue / _exclusiveUpperBound),
            Hour: From(newHourValue % _exclusiveUpperBound)
        );
    }

    public int CompareTo(IHour? other) =>
        other is null ?
            CompareToResult.WhenOtherIsNull :
            CompareTo(From(other));

    public int CompareTo(HourFormat24? other) =>
        other is null ?
            CompareToResult.WhenOtherIsNull :
            Value.CompareTo(other.Value);

    public TimeOnly ToTimeOnly() => new(hour: Value, minute: 0);

    public int ToInt() => Value;

    public string ToShortString() => $"{Value:00}";

    private static readonly NumberRange<int> _range = NumberRange<int>.FromFiniteInclusive(0, 23);
    private static readonly int _exclusiveUpperBound = 24;

    public static HourFormat24 From(IHour hour) => hour switch
    {
        HourFormat24 hour24 => hour24,
        HourFormat12 hour12 => From(hour12),
        _ => throw new NotSupportedException($"Could not construct {nameof(HourFormat24)} instance. Type {hour.GetType().Name} was unexpected"),
    };

    public static HourFormat24 From(HourFormat12 hour12)
    {
        int hourValue = hour12.Value;

        if (hour12.DayPeriod == DayPeriod.AM)
        {
            if (hourValue == 12)
            {
                hourValue = 0; // Convert 12 AM to 0 (midnight)
            }
            return new() { Value = hourValue };
        }

        if (hourValue != 12)
        {
            hourValue += 12; // Convert 1-11 PM to 13-23
        }
        return new() { Value = hourValue };
    }

    public static HourFormat24 From(int hour)
    {
        _range.ThrowIfDoesNotContain(hour);
        return new() { Value = hour, };
    }

    private HourFormat24() { }
}
