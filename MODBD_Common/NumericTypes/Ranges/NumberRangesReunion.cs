using System.Collections.ObjectModel;
using System.Numerics;

namespace MODBD_Common.NumericTypes.Ranges;

public sealed class NumberRangesReunion<TNumber> :
    ReadOnlyCollection<NumberRange<TNumber>>,
    INumberSet<TNumber>,
    IReadOnlyList<NumberRange<TNumber>>,
    IEquatable<NumberRangesReunion<TNumber>>
    where TNumber : struct, INumber<TNumber>, IMinMaxValue<TNumber>
{
    public bool Equals(NumberRangesReunion<TNumber>? other) =>
        other is not null && 
        Count == other.Count && 
        !this.Any((NumberRange<TNumber> range) =>
            ! other.Contains(range)
        );

    public NumberRangesReunion<TNumber> Intersect(NumberRangesReunion<TNumber> other) => From(
        other   // for each other range, create an intersections with my ranges
            .SelectMany(IntersectAsIEnumerable) // then flat map them to reunite them
                                                // (IntersectAsIEnumerable returns a reunion, but we can still find ranges to reunite from different reunions, that's why we flat map them to apply From on top of all)
    );

    public NumberRangesReunion<TNumber> Intersect(NumberRange<TNumber> other) => new([.. IntersectAsIEnumerable(other)]);

    private IEnumerable<NumberRange<TNumber>> IntersectAsIEnumerable(NumberRange<TNumber> other) =>
        this
            .Select(range => range.Intersect(other))
            .OfType<NumberRange<TNumber>>();

    public NumberRangesReunion<TNumber> Reunite(NumberRangesReunion<TNumber> other) =>
        From([.. this, .. other]);

    public NumberRangesReunion<TNumber> Reunite(NumberRange<TNumber> other) =>
        From([other, .. this]);

    public static readonly NumberRangesReunion<TNumber> NullNumberSet = new([]) { };

    /// <summary>
    /// Creates a new instance of <see cref="NumberRangesReunion{TNumber}"/> from a multiset of ranges. <br></br>
    /// </summary>
    /// <param name="sourceRanges"></param>
    /// <returns></returns>
    public static NumberRangesReunion<TNumber> From(IEnumerable<NumberRange<TNumber>> sourceRanges) =>
        From([.. sourceRanges]);

    /// <summary>
    /// Creates a new instance of <see cref="NumberRangesReunion{TNumber}"/> from a set of ranges. <br></br>
    /// </summary>
    /// <param name="sourceRanges"></param>
    /// <returns></returns>
    public static NumberRangesReunion<TNumber> From(HashSet<NumberRange<TNumber>> sourceRanges)
    {
        for (int i = 0; i < sourceRanges.Count; i++)
        {
            NumberRange<TNumber> range1 = sourceRanges.ElementAt(i);

            for (int j = 0; j < i; j++)
            {
                NumberRange<TNumber> range2 = sourceRanges.ElementAt(j);

                if (range1.Intersect(range2) is NumberRange<TNumber>)
                {
                    sourceRanges.Remove(range1);
                    sourceRanges.Remove(range2);

                    NumberRange<TNumber> reunitedRange = NumberRange<TNumber>.From(
                        LowerBoundMin.Of(range1.LowerBound, range2.LowerBound),
                        UpperBoundMax.Of(range1.UpperBound, range2.UpperBound)
                    );

                    sourceRanges.Add(reunitedRange);
                    i = 0;
                    break;
                }
            }
        }

        return new([.. sourceRanges]);
    }

    private NumberRangesReunion(IList<NumberRange<TNumber>> source) : base(source) { }
}
