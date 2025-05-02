using MODBD_Core.Schema;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace MODBD_Core.Applications.SqlConditions;

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

    [SetsRequiredMembers]
    private ConditionWithOtherColumn(
        Column column1,
        Operator @operator,
        Column column2
    )
    {
        Column1 = column1;
        Operator = @operator;
        Column2 = column2;

        Columns = [Column1, Column2];
        Sql = $"{Column1.ToSql()} {Operator.Name} {Column2.ToSql()}";
    }
}
