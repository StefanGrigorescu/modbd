using MODBD_Common.NullableTypes;
using MODBD_Common.Time.DateTimes;
using MODBD_Common.Time.DayHourSpans;

namespace MODBD_Common.Time.Ranges;

public sealed class DateTimeRange<TDateTime>
    where TDateTime : class, IDateTime<TDateTime>, IComparable<TDateTime>
{
    public required TDateTime Start { get; init; }
    public required TDateTime End { get; init; }

    /// <summary>
    /// Shifts the both <see cref="Start"/> and <see cref="End"/> dates into the future by the specified <paramref name="modifier"/>.
    /// </summary>
    /// <param name="modifier">A number of whole days and hours. The parameter can be only positive.</param>
    /// <returns>
    /// A new <see cref="TDateTimeRange"/> instance <br></br>
    /// whose <see cref="Start"/> is represented by the sum of this instance's <see cref="Start"/> and the number of days and hours represented by <paramref name="modifier"/> <br></br>
    /// and whose <see cref="End"/> is represented by the sum of this instance's <see cref="End"/> and the number of days and hours represented by <paramref name="modifier"/>.
    /// </returns>
    public DateTimeRange<TDateTime> Shift(DayHourSpan modifier) =>
        From(
            Start.Add(modifier),
            End.Add(modifier)
        );

    /// <summary>
    /// Extends the <see cref="End"/> date into the future by the specified <paramref name="modifier"/>. <br></br>
    /// The operation will only extend the <see cref="End"/> date while keeping the <see cref="Start"/> date unchanged.
    /// </summary>
    /// <param name="modifier">A number of whole days and hours. The parameter can be only positive.</param>
    /// <returns>
    /// A new <see cref="TDateTimeRange"/> instance <br></br>
    /// whose <see cref="Start"/> is represented by this instance's <see cref="Start"/> <br></br>
    /// and whose <see cref="End"/> is represented by the sum of this instance's <see cref="End"/> and the number of days and hours represented by <paramref name="modifier"/>.
    /// </returns>
    public DateTimeRange<TDateTime> Extend(DayHourSpan modifier) =>
        From(
            Start,
            End.Add(modifier)
        );

    /// <summary>
    /// Anticipates the <see cref="Start"/> date into the future by the specified <paramref name="modifier"/>. <br></br>
    /// The operation will only move the <see cref="Start"/> date while keeping the <see cref="End"/> date unchanged.
    /// </summary>
    /// <param name="modifier">A number of whole days and hours. The parameter can be only positive.</param>
    /// <returns>
    /// A new <see cref="TDateTimeRange"/> instance <br></br>
    /// whose <see cref="Start"/> is represented by the difference between this instance's <see cref="Start"/> and the number of days and hours represented by <paramref name="modifier"/> <br></br>
    /// and whose <see cref="End"/> is represented by this instance's <see cref="End"/>.
    /// </returns>
    public DateTimeRange<TDateTime> StartEarlier(DayHourSpan modifier) =>
        From(
            Start.Substract(modifier),
            End
        );

    /// <summary>
    /// Anticipates the <see cref="End"/> date into the future by the specified <paramref name="modifier"/>. <br></br>
    /// The operation will only move the <see cref="End"/> date while keeping the <see cref="Start"/> date unchanged.
    /// </summary>
    /// <param name="modifier">A number of whole days and hours. The parameter can be only positive.</param>
    /// <returns>
    /// A new <see cref="TDateTimeRange"/> instance <br></br>
    /// whose <see cref="Start"/> is represented by this instance's <see cref="Start"/> <br></br>
    /// whose <see cref="End"/> is represented by the difference between this instance's <see cref="End"/> and the number of days and hours represented by <paramref name="modifier"/>.
    /// </returns>
    public DateTimeRange<TDateTime> EndEarlier(DayHourSpan modifier) =>
        From(
            Start,
            End.Substract(modifier)
        );

    /// <summary>
    /// Checks if <see cref="Start"/> is earlier than <paramref name="otherDateTime"/>. <br></br>
    /// If the <see cref="Start"/> is null, it returns false. 
    /// </summary>
    /// <param name="otherDateTime">The dateTime to be compared to the <see cref="Start"/>.</param>
    /// <returns>True if the <see cref="Start"/> is not null and is earlier than <paramref name="otherDateTime"/>, or false otherwise.</returns>
    public bool IsStartEarlierThan(TDateTime otherDateTime) =>
        Start.CompareTo(otherDateTime) < 0;

    /// <summary>
    /// Checks if <see cref="Start"/> is later than <paramref name="otherDateTime"/>. <br></br>
    /// If the <see cref="Start"/> is null, it returns false. 
    /// </summary>
    /// <param name="otherDateTime">The dateTime to be compared to the <see cref="Start"/>.</param>
    /// <returns>True if the <see cref="Start"/> is not null and is later than <paramref name="otherDateTime"/>, or false otherwise.</returns>
    public bool IsStartLaterThan(TDateTime otherDateTime) =>
        Start.CompareTo(otherDateTime) > 0;

    /// <summary>
    /// Checks if <see cref="End"/> is earlier than <paramref name="otherDateTime"/>. <br></br>
    /// If the <see cref="End"/> is null, it returns false. 
    /// </summary>
    /// <param name="otherDateTime">The dateTime to be compared to the <see cref="End"/>.</param>
    /// <returns>True if the <see cref="End"/> is not null and is earlier than <paramref name="otherDateTime"/>, or false otherwise.</returns>
    public bool IsEndEarlierThan(TDateTime otherDateTime) =>
        End.CompareTo(otherDateTime) < 0;

    /// <summary>
    /// Checks if <see cref="End"/> is later than <paramref name="otherDateTime"/>. <br></br>
    /// If the <see cref="End"/> is null, it returns false. 
    /// </summary>
    /// <param name="otherDateTime">The dateTime to be compared to the <see cref="End"/>.</param>
    /// <returns>True if the <see cref="End"/> is not null and is later than <paramref name="otherDateTime"/>, or false otherwise.</returns>
    public bool IsEndLaterThan(TDateTime otherDateTime) =>
        End.CompareTo(otherDateTime) > 0;

    public static DateTimeRange<TDateTime> From(TDateTime? start, TDateTime? end)
    {
        TDateTime startDate = start.ValidateNotNull();
        TDateTime endDate = end.ValidateNotNull();
        DateTimeRangeException<TDateTime>.ThrowIfIsStartAfterEnd(startDate, endDate);
        return new()
        {
            Start = startDate,
            End = endDate,
        };
    }

    private DateTimeRange() { }
}
