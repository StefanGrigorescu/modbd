namespace MODBD_Api.Common.Time.Ranges;

public sealed class IncompleteDateTimeRangeException : Exception
{
    public static void ThrowIfIsIncompleteDateTimeRange(DateTime? start, DateTime? end)
    {
        if (start is null && end is not null)
        {
            throw new IncompleteDateTimeRangeException(start, end);
        }
        if (start is not null && end is null)
        {
            throw new IncompleteDateTimeRangeException(start, end);
        }
    }

    private IncompleteDateTimeRangeException(DateTime? start, DateTime? end) : base(
        $"You need to define both start and end moments for this time period! Actual range is {start} - {end}."
    ) { }
}
