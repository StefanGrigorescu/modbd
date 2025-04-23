using System.Diagnostics;
using MODBD_Common.Abstractions.DiscriminatedUnions;
using MODBD_Common.NullableTypes;

namespace MODBD_Common.Time.Hours;

public sealed class DayPeriod : Enumeration<DayPeriod>, IComparable<DayPeriod>
{
    public static readonly DayPeriod AM = new() { Id = 0, Name = "AM", };
    public static readonly DayPeriod PM = new() { Id = 1, Name = "PM", };

    public int CompareTo(DayPeriod? other)
    {
        if (other is null)
        {
            return CompareToResult.WhenOtherIsNull;
        }
        if (this == other)
        {
            return 0;
        }
        if (this == AM && other == PM)
        {
            return -1;
        }
        if (this == PM && other == AM)
        {
            return 1;
        }
        throw new UnreachableException($"Attempting to compare unexpected Day Periods: {this} and {other}");
    }

    private DayPeriod() { }
}
