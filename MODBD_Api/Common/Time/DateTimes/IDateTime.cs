using MODBD_Api.Common.Time.DayHourSpans;

namespace MODBD_Api.Common.Time.DateTimes;

public interface IDateTime<TDateTime>
    where TDateTime : class, IDateTime<TDateTime>, IComparable<TDateTime>
{
    TDateTime Add(DayHourSpan modifier);
    TDateTime Substract(DayHourSpan modifier);
    TimeSpan Substract(TDateTime other);
    DateTime ToDateTime();
}
