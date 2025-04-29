using MODBD_Common.NumericTypes.Ranges;
using MODBD_Core.Schema;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace MODBD_Core.Applications.SqlConditions;

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
    )
    {
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
