using MODBD_Common.NumericTypes.Positive;

namespace MODBD_Common.Time.DayHourSpans;

public sealed record DayHourSpan
{
    public required Positive<int> Days { get; init; }
    public required Positive<int> Hours { get; init; }

    public static DayHourSpan From(IDayHourSpanDto source) => new()
    {
        Days = Positive<int>.From(source.Days),
        Hours = Positive<int>.From(source.Hours),
    };
    private DayHourSpan() { }
}
