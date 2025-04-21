using MODBD_Analiza.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace MODBD_Analiza.Applications;

internal interface IApplication
{
    string Sql { get; }
    IReadOnlyList<Column> AllColumns { get; }
}


internal sealed record Application<TTable> : IApplication
    where TTable : Table<TTable>
{
    public required Func<TTable, IReadOnlyList<Column>> Select { get; init; }
    public required TTable Table { get; init; }
    public required Func<TTable, IReadOnlyList<ConditionWithValue>> Where { get; init; }
    public required IReadOnlyList<Column> SelectColumns { get; init; }
    public required IReadOnlyList<Column> WhereColumns { get; init; }
    public required IReadOnlyList<Column> AllColumns { get; init; }
    public required string Sql { get; init; }

    public static Application<TTable> New(Func<TTable, IReadOnlyList<Column>> select, TTable table, Func<TTable, IReadOnlyList<ConditionWithValue>> where) =>
        new(select, table, where);

    [SetsRequiredMembers]
    private Application(
        Func<TTable, IReadOnlyList<Column>> select,
        TTable table,
        Func<TTable, IReadOnlyList<ConditionWithValue>> where
    )
    {
        Select = select;
        Table = table;
        Where = where;

        IReadOnlyList<ConditionWithValue> whereConditions = Where(table);

        SelectColumns = Select(table)
            .Distinct();
        WhereColumns = whereConditions
            .Select(cond => cond.Column)
            .Distinct();
        AllColumns = SelectColumns
            .Concat(WhereColumns)
            .Distinct();

        Sql = new SqlQueryBuilder()
            .Select(SelectColumns)
            .From(table)
            .Where(whereConditions)
            .ToSql();
    }
}


internal sealed record Application<TTable1, TTable2> : IApplication
    where TTable1 : Table<TTable1>
    where TTable2 : Table<TTable2>
{
    public required Func<TTable1, TTable2, IReadOnlyList<Column>> Select { get; init; }
    public required AliasedTable<TTable1> Table1 { get; init; }
    public required AliasedTable<TTable2> Table2 { get; init; }
    public required Func<TTable1, TTable2, IReadOnlyList<ConditionWithOtherColumn>> On { get; init; }
    public required Func<TTable1, TTable2, IReadOnlyList<ICondition>> Where { get; init; }
    public required IReadOnlyList<Column> SelectColumns { get; init; }
    public required IReadOnlyList<Column> WhereColumns { get; init; }
    public required IReadOnlyList<Column> OnColumns { get; init; }
    public required IReadOnlyList<Column> AllColumns { get; init; }
    public required string Sql { get; init; }

    public static Application<TTable1, TTable2> New(
        Func<TTable1, TTable2, IReadOnlyList<Column>> select,
        AliasedTable<TTable1> table1,
        AliasedTable<TTable2> table2,
        Func<TTable1, TTable2, IReadOnlyList<ConditionWithOtherColumn>> on,
        Func<TTable1, TTable2, IReadOnlyList<ICondition>> where
    ) => new(select, table1, table2, on, where);

    [SetsRequiredMembers]
    private Application(
        Func<TTable1, TTable2, IReadOnlyList<Column>> select,
        AliasedTable<TTable1> table1,
        AliasedTable<TTable2> table2,
        Func<TTable1, TTable2, IReadOnlyList<ConditionWithOtherColumn>> on,
        Func<TTable1, TTable2, IReadOnlyList<ICondition>> where
    )
    {
        Select = select;
        Table1 = table1;
        Table2 = table2;
        On = on;
        Where = where;

        IReadOnlyList<ConditionWithOtherColumn> onConditions = On(table1, table2);
        IReadOnlyList<ICondition> whereConditions = Where(table1, table2);

        SelectColumns = Select(table1, table2)
            .Distinct();
        OnColumns = onConditions
            .SelectMany(cond => cond.Columns)
            .Distinct();
        WhereColumns = whereConditions
            .SelectMany(cond => cond.Columns)
            .Distinct();
        AllColumns = SelectColumns
            .Concat(OnColumns)
            .Concat(WhereColumns)
            .Distinct();

        Sql = new SqlQueryBuilder()
            .Select(SelectColumns)
            .From(table1)
            .InnerJoin(table2)
            .On(onConditions)
            .Where(whereConditions)
            .ToSql();
    }
}


internal class SqlQueryBuilder
{
    private readonly StringBuilder _stringBuilder = new();

    public SqlQueryBuilder Select(IReadOnlyList<Column> selectColumns)
    {
        _stringBuilder.AppendLine($"SELECT {string.Join(", ", selectColumns.Select(c => c.ToSql()))} ");
        return this;
    }

    public SqlQueryBuilder From(ITable table)
    {
        _stringBuilder.AppendLine($"FROM {table.ToSql()} ");
        return this;
    }

    public SqlQueryBuilder InnerJoin(ITable table)
    {
        _stringBuilder.AppendLine($"INNER JOIN {table.ToSql()} ");
        return this;
    }

    public SqlQueryBuilder On(IReadOnlyList<ConditionWithOtherColumn> onConditions)
    {
        _stringBuilder.AppendLine($"\tON {string.Join("\n\tAND ", onConditions.Select(c => c.Sql))} ");
        return this;
    }

    public SqlQueryBuilder Where(IReadOnlyList<ICondition> whereConditions)
    {
        _stringBuilder.AppendLine($"WHERE {string.Join("\n\tAND ", whereConditions.Select(c => c.Sql))}");
        return this;
    }

    public string ToSql() => _stringBuilder
        .ToString()
        .TrimEnd();
}
