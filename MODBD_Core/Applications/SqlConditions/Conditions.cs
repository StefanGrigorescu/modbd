using MODBD_Common.Collections;
using MODBD_Common.NumericTypes.Ranges;
using MODBD_Core.Applications;
using MODBD_Core.Schema;
using System.Numerics;

namespace MODBD_Core.Applications.SqlConditions;

public sealed class Conditions :
    SortedList<int, ICondition>,
    IReadOnlyDictionary<int, ICondition>,
    IEquatable<Conditions>
{
    public IReadOnlyList<Column> DistinctColumns() =>
        this.Select(kvp =>
            (kvp.Key, kvp.Value.Columns)
        ).DistinctBy(kvp => kvp.Columns)
        .OrderBy(kvp => kvp.Key)
        .SelectMany(kvp => kvp.Columns)
        .ToIReadOnlyList();

    public bool Equals(Conditions? other)
    {
        if(other is null || Count != other.Count)
        {
            return false;
        }

        return !this.Any((kvp) =>                         // any key 
            !other.TryGetValue(kvp.Key, out ICondition? otherCondition) ||      // not found in other dictionary
                !kvp.Value.Equals(otherCondition)                                               // or with different value
        );
    }

    public override bool Equals(object? obj) =>
        obj is not null &&
        obj is Conditions other && 
        Equals(other);

    public override int GetHashCode()
    {
        int hash = 17;
        foreach (KeyValuePair<int, ICondition> kvp in this)
        {
            hash = hash * 31 + kvp.Key; // HashCode of the condition
        }
        return hash;
    }

    public string ToSql() => string.Join("  &  ", Values.Select(condition => condition.Sql));

    public (Conditions SimplifiedConditions, bool HadChanged) Simplify()
    {
        List<ConditionWithValue<int>> conditionsWithValues = Values.OfType<ConditionWithValue<int>>().ToList();
        List<ICondition> simplifiedConditions = Values.Except(conditionsWithValues).ToList();

        // Use a HashSet to track indices of conditions that should be removed
        HashSet<int> indicesToRemove = new();

        for (int i = 0; i < conditionsWithValues.Count; i++)
        {
            if (indicesToRemove.Contains(i))
            {
                continue;
            }

            ConditionWithValue<int> condition = conditionsWithValues[i];

            for (int j = i + 1; j < conditionsWithValues.Count; j++)
            {
                if (indicesToRemove.Contains(j)) 
                { 
                    continue; 
                }

                ConditionWithValue<int> otherCondition = conditionsWithValues[j];
                if(! condition.Column.Equals(otherCondition.Column))
                {
                    // if the columns are not equal, we can not simplify them
                    continue;
                }

                NumberRangesReunion<int> intersection = condition.Codomain.Intersect(otherCondition.Codomain);
                if (intersection.Equals(condition.Codomain))
                {
                    // condition codomain is a subset of otherCondition codomain (is stricter),
                    // so we can remove otherCondition as it provides no extra constraints
                    indicesToRemove.Add(j);
                }
                else if (intersection.Equals(otherCondition.Codomain))
                {
                    // otherCondition codomain is a subset of condition codomain (is stricter),
                    // so we can remove condition as it provides no extra constraints
                    indicesToRemove.Add(i);
                    break;
                }
            }

            // At this point, the condition was checked and it can not be simplified against other conditions, 
            // so we can add it to the candidate solution
            simplifiedConditions.Add(condition);
        }

        IEnumerable<ConditionWithValue<int>> conditionsToKeep = conditionsWithValues.Where((ConditionWithValue<int> _, int idx) =>
            ! indicesToRemove.Contains(idx)
        );
        simplifiedConditions.AddRange(conditionsToKeep);

        return (new(simplifiedConditions), indicesToRemove.Count != 0);
    }

    public Conditions And(ICondition other) =>
        new([.. Values, other]);

    public Conditions And(Conditions others) =>
        new([.. Values, .. others.Values]);

    public Conditions(IList<ICondition> list) : base(
        list.ToSortedList(cond => cond.GetHashCode())
    ) { }

    public Conditions(IEnumerable<ICondition> list) : base(
        list.ToSortedList(cond => cond.GetHashCode())
    ) { }
}


public static class ConditionsBuilder
{
    public static Conditions LessThan<TSqlValue>(this Column column, TSqlValue value)
        where TSqlValue : struct, IComparable<TSqlValue>, IEquatable<TSqlValue>, INumber<TSqlValue>, IMinMaxValue<TSqlValue> =>
        ConditionWithValue<TSqlValue>.NewList(column, Operator.LessThan, SqlValue<TSqlValue>.From(value));

    public static Conditions LessThanOrEqual<TSqlValue>(this Column column, TSqlValue value)
        where TSqlValue : struct, IComparable<TSqlValue>, IEquatable<TSqlValue>, INumber<TSqlValue>, IMinMaxValue<TSqlValue> =>
        ConditionWithValue<TSqlValue>.NewList(column, Operator.LessThanOrEqual, SqlValue<TSqlValue>.From(value));

    public static Conditions GreaterThan<TSqlValue>(this Column column, TSqlValue value)
        where TSqlValue : struct, IComparable<TSqlValue>, IEquatable<TSqlValue>, INumber<TSqlValue>, IMinMaxValue<TSqlValue> =>
        ConditionWithValue<TSqlValue>.NewList(column, Operator.GreaterThan, SqlValue<TSqlValue>.From(value));

    public static Conditions GreaterThanOrEqual<TSqlValue>(this Column column, TSqlValue value)
        where TSqlValue : struct, IComparable<TSqlValue>, IEquatable<TSqlValue>, INumber<TSqlValue>, IMinMaxValue<TSqlValue> =>
        ConditionWithValue<TSqlValue>.NewList(column, Operator.GreaterThanOrEqual, SqlValue<TSqlValue>.From(value));

    public static Conditions Equal<TSqlValue>(this Column column, TSqlValue value)
        where TSqlValue : struct, IComparable<TSqlValue>, IEquatable<TSqlValue>, INumber<TSqlValue>, IMinMaxValue<TSqlValue> =>
        ConditionWithValue<TSqlValue>.NewList(column, Operator.Equal, SqlValue<TSqlValue>.From(value));

    public static Conditions NotEqual<TSqlValue>(this Column column, TSqlValue value)
        where TSqlValue : struct, IComparable<TSqlValue>, IEquatable<TSqlValue>, INumber<TSqlValue>, IMinMaxValue<TSqlValue> =>
        ConditionWithValue<TSqlValue>.NewList(column, Operator.NotEqual, SqlValue<TSqlValue>.From(value));


    public static Conditions LessThan(this Column column1, Column column2) =>
        ConditionWithOtherColumn.NewList(column1, Operator.LessThan, column2);

    public static Conditions LessThanOrEqual(this Column column1, Column column2) =>
        ConditionWithOtherColumn.NewList(column1, Operator.LessThanOrEqual, column2);

    public static Conditions GreaterThan(this Column column1, Column column2) =>
        ConditionWithOtherColumn.NewList(column1, Operator.GreaterThan, column2);

    public static Conditions GreaterThanOrEqual(this Column column1, Column column2) =>
        ConditionWithOtherColumn.NewList(column1, Operator.GreaterThanOrEqual, column2);

    public static Conditions Equal(this Column column1, Column column2) =>
        ConditionWithOtherColumn.NewList(column1, Operator.Equal, column2);

    public static Conditions NotEqual(this Column column1, Column column2) =>
        ConditionWithOtherColumn.NewList(column1, Operator.NotEqual, column2);
}
