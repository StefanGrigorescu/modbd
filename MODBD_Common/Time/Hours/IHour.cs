using MODBD_Common.NumericTypes.Positive;

namespace MODBD_Common.Time.Hours;

public interface IHour : IComparable<IHour>
{
    (Positive<int> DaySpan, IHour Hour) AddHours(Positive<int> hours);
    (Positive<int> DaySpan, IHour Hour) SubstractHours(Positive<int> hours);
    TimeOnly ToTimeOnly();
    int ToInt();
    string ToShortString();
}
