using MODBD_Common.Collections;
using MODBD_Common.NumericTypes.Ranges;
using MODBD_Core.Schema;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace MODBD_Core.Applications;

// TODO: Implement ConditionWithSqlParameter


[DebuggerDisplay("{Column} {Operator} {Value}")]
public sealed record ConditionWithValue<TSqlValue> : ICondition
    where TSqlValue : struct, IComparable<TSqlValue>, IEquatable<TSqlValue>, INumber<TSqlValue>, IMinMaxValue<TSqlValue>
{
    public required Column Column { get; init; }
    public required Operator Operator { get; init; }
    public required SqlValue<TSqlValue> Value { get; init; }

    public required IReadOnlyList<Column> Columns { get; init; }
    public required string Sql { get; init; }
    public required NumberRangesReunion<TSqlValue> Codomain { get; init; } = NumberRangesReunion<TSqlValue>.NullNumberSet;

    public ICondition Not() =>
        new ConditionWithValue<TSqlValue>(Column, Operator.Not(Operator), Value);

    public static Conditions NewList(Column column, Operator @operator, SqlValue<TSqlValue> value) =>
        new([new ConditionWithValue<TSqlValue>(column, @operator, value)]);

    public IReadOnlyList<ICondition> And(ICondition other) =>
        [this, other];

    public bool Equals(ICondition? other) => 
        other is not null &&
        other is ConditionWithValue<TSqlValue> otherCondition &&
        Column.Equals(otherCondition.Column) &&
        Operator.Equals(otherCondition.Operator) &&
        Value.Equals(otherCondition.Value);

    public override int GetHashCode() =>
        HashCode.Combine(Column, Operator, Value);

    [SetsRequiredMembers]
    private ConditionWithValue(
        Column column,
        Operator @operator,
        SqlValue<TSqlValue> value
    ) {
        Column = column;
        Operator = @operator;
        Value = value;
        Codomain = ConditionCodomain.From(@operator, value);

        Columns = [Column];
        Sql = $"{Column.ToSql()} {Operator.Name} {Value}";
    }
}

public static class ConditionCodomain
{
    public static NumberRangesReunion<TSqlValue> From<TSqlValue>(
        Operator @operator,
        SqlValue<TSqlValue> value
    )
        where TSqlValue : struct, IComparable<TSqlValue>, IEquatable<TSqlValue>, INumber<TSqlValue>, IMinMaxValue<TSqlValue>
     => @operator.Match<NumberRangesReunion<TSqlValue>>(new()
     {
         LessThan = _ => NumberRange<TSqlValue>.From(
             MinusInfinity<TSqlValue>.Instance,
             UpperBound<TSqlValue>.Exclusive.From(value)
         ).AsReunion(),

         LessThanOrEqual = _ => NumberRange<TSqlValue>.From(
             MinusInfinity<TSqlValue>.Instance,
             UpperBound<TSqlValue>.Inclusive.From(value)
         ).AsReunion(),

         GreaterThan = _ => NumberRange<TSqlValue>.From(
             LowerBound<TSqlValue>.Exclusive.From(value),
             Infinity<TSqlValue>.Instance
         ).AsReunion(),

         GreaterThanOrEqual = _ => NumberRange<TSqlValue>.From(
             LowerBound<TSqlValue>.Inclusive.From(value),
             Infinity<TSqlValue>.Instance
         ).AsReunion(),

         Equal = _ => NumberRange<TSqlValue>.From(
             LowerBound<TSqlValue>.Inclusive.From(value),
             UpperBound<TSqlValue>.Inclusive.From(value)
         ).AsReunion(),

         NotEqual = _ => NumberRangesReunion<TSqlValue>.From([
             NumberRange<TSqlValue>.From(
                 MinusInfinity<TSqlValue>.Instance,
                 UpperBound<TSqlValue>.Exclusive.From(value)
             ),
             NumberRange<TSqlValue>.From(
                 LowerBound<TSqlValue>.Exclusive.From(value),
                 Infinity<TSqlValue>.Instance
             )
         ]),
     });
}


[DebuggerDisplay("{Column1} {Operator} {Column2}")]
public sealed record ConditionWithOtherColumn : ICondition
{
    public required Column Column1 { get; init; }
    public required Operator Operator { get; init; }
    public required Column Column2 { get; init; }

    public required IReadOnlyList<Column> Columns { get; init; }
    public required string Sql { get; init; }

    public ICondition Not() =>
        new ConditionWithOtherColumn(Column1, Operator.Not(Operator), Column2);

    public static Conditions NewList(Column column1, Operator @operator, Column column2) =>
        new([new ConditionWithOtherColumn(column1, @operator, column2)]);

    public IReadOnlyList<ICondition> And(ICondition other) =>
        [this, other];

    public bool Equals(ICondition? other) =>
        other is not null &&
        other is ConditionWithOtherColumn otherCondition &&
        Column1.Equals(otherCondition.Column1) &&
        Operator.Equals(otherCondition.Operator) &&
        Column2.Equals(otherCondition.Column2);

    public override int GetHashCode() =>
        HashCode.Combine(Column1, Operator, Column2);

    [SetsRequiredMembers] private ConditionWithOtherColumn(
        Column column1,
        Operator @operator,
        Column column2
    ) {
        Column1 = column1;
        Operator = @operator;
        Column2 = column2;

        Columns = [Column1, Column2];
        Sql = $"{Column1.ToSql()} {Operator.Name} {Column2.ToSql()}";
    }
}


public interface ICondition : IEquatable<ICondition>
{
    IReadOnlyList<Column> Columns { get; }
    string Sql { get; }
    ICondition Not();
}


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

        return !this.Any((KeyValuePair<int, ICondition> kvp) =>                         // any key 
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
