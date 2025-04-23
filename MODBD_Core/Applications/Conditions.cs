using MODBD_Core.Schema;
using System.Diagnostics.CodeAnalysis;

namespace MODBD_Core.Applications;

public sealed record ConditionWithValue : ICondition
{
    public required Column Column { get; init; }
    public required Operator Operator { get; init; }
    public required object Value { get; init; }

    public required IReadOnlyList<Column> Columns { get; init; }
    public required string Sql { get; init; }

    public static IReadOnlyList<ConditionWithValue> NewList(Column column, Operator @operator, object value) =>
        [new ConditionWithValue(column, @operator, value)];

    public IReadOnlyList<ICondition> And(ICondition other) =>
        [this, other];

    [SetsRequiredMembers]
    private ConditionWithValue(
        Column column,
        Operator @operator,
        object value
    ) {
        Column = column;
        Operator = @operator;
        Value = value;

        Columns = [Column];
        Sql = $"{Column.ToSql()} {Operator.Name} {Value}";
    }
}

public sealed record ConditionWithOtherColumn : ICondition
{
    public required Column Column1 { get; init; }
    public required Operator Operator { get; init; }
    public required Column Column2 { get; init; }

    public required IReadOnlyList<Column> Columns { get; init; }
    public required string Sql { get; init; }

    public static IReadOnlyList<ConditionWithOtherColumn> NewList(Column column1, Operator @operator, Column column2) =>
        [new ConditionWithOtherColumn(column1, @operator, column2)];

    public IReadOnlyList<ICondition> And(ICondition other) =>
        [this, other];

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


public interface ICondition
{
    IReadOnlyList<Column> Columns { get; }
    string Sql { get; }
}


public static class ConditionsBuilder
{
    public static IReadOnlyList<ConditionWithValue> LessThan(this Column column, object value) =>
        ConditionWithValue.NewList(column, Operator.LessThan, value);

    public static IReadOnlyList<ConditionWithValue> LessThanOrEqual(this Column column, object value) =>
        ConditionWithValue.NewList(column, Operator.LessThanOrEqual, value);

    public static IReadOnlyList<ConditionWithValue> GreaterThan(this Column column, object value) =>
        ConditionWithValue.NewList(column, Operator.GreaterThan, value);

    public static IReadOnlyList<ConditionWithValue> GreaterThanOrEqual(this Column column, object value) =>
        ConditionWithValue.NewList(column, Operator.GreaterThanOrEqual, value);

    public static IReadOnlyList<ConditionWithValue> Equal(this Column column, object value) =>
        ConditionWithValue.NewList(column, Operator.Equal, value);

    public static IReadOnlyList<ConditionWithValue> NotEqual(this Column column, object value) =>
        ConditionWithValue.NewList(column, Operator.NotEqual, value);


    public static IReadOnlyList<ConditionWithOtherColumn> LessThan(this Column column1, Column column2) =>
        ConditionWithOtherColumn.NewList(column1, Operator.LessThan, column2);

    public static IReadOnlyList<ConditionWithOtherColumn> LessThanOrEqual(this Column column1, Column column2) =>
        ConditionWithOtherColumn.NewList(column1, Operator.LessThanOrEqual, column2);

    public static IReadOnlyList<ConditionWithOtherColumn> GreaterThan(this Column column1, Column column2) =>
        ConditionWithOtherColumn.NewList(column1, Operator.GreaterThan, column2);

    public static IReadOnlyList<ConditionWithOtherColumn> GreaterThanOrEqual(this Column column1, Column column2) =>
        ConditionWithOtherColumn.NewList(column1, Operator.GreaterThanOrEqual, column2);

    public static IReadOnlyList<ConditionWithOtherColumn> Equal(this Column column1, Column column2) =>
        ConditionWithOtherColumn.NewList(column1, Operator.Equal, column2);

    public static IReadOnlyList<ICondition> NotEqual(this Column column1, Column column2) =>
        ConditionWithOtherColumn.NewList(column1, Operator.NotEqual, column2);
}


public static class Conditions
{
    public static IReadOnlyList<ICondition> And(this IReadOnlyList<ICondition> conditions, ConditionWithValue other) =>
        [.. conditions, other];

    public static IReadOnlyList<ICondition> And(this IReadOnlyList<ICondition> conditions, ConditionWithOtherColumn other) =>
        [.. conditions, other];
}
