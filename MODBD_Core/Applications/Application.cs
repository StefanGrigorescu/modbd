using MODBD_Common.Collections;
using MODBD_Common.NumericTypes.Positive;
using MODBD_Common.Text;
using MODBD_Core.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace MODBD_Core.Applications;

public interface IApplication
{
    TextFieldSm Name { get; }
    Positive<double> FrequencyPerMonth { get; }
    Positive<int> Selectivity { get; }
    string Sql { get; }
    IReadOnlyList<Column> AllColumns { get; }
    Conditions WhereConditions { get; }
}


public sealed record Application<TTable> : IApplication
    where TTable : Table<TTable>
{
    public required TextFieldSm Name { get; init; }
    public required Positive<double> FrequencyPerMonth { get; init; }
    public required Positive<int> Selectivity { get; init; }
    public required Func<TTable, IReadOnlyList<Column>> Select { get; init; }
    public required TTable Table { get; init; }
    public required Func<TTable, Conditions> Where { get; init; }
    public required Conditions WhereConditions { get; init; }
    public required IReadOnlyList<Column> SelectColumns { get; init; }
    public required IReadOnlyList<Column> WhereColumns { get; init; }
    public required IReadOnlyList<Column> AllColumns { get; init; }
    public required string Sql { get; init; }

    public Application<TTable> WithFrequencyPerMonth(double frequencyPerMonth) =>
        WithFrequencyPerMonth(Positive<double>.From(frequencyPerMonth));

    public Application<TTable> WithFrequencyPerMonth(Positive<double> frequencyPerMonth) =>
        this with { FrequencyPerMonth = frequencyPerMonth, };

    public Application<TTable> WithSelectivity(int selectivity) =>
        WithSelectivity(Positive<int>.From(selectivity));

    public Application<TTable> WithSelectivity(Positive<int> selectivity) =>
        this with { Selectivity = selectivity, };

    public Application<TTable> WithName(string name) =>
        WithName(TextFieldSm.From(name));

    public Application<TTable> WithName(TextFieldSm name) =>
        this with { Name = name, };

    public static Application<TTable> New(Func<TTable, IReadOnlyList<Column>> select, TTable table, Func<TTable, Conditions> where) =>
        new(select, table, where);

    [SetsRequiredMembers]
    private Application(
        Func<TTable, IReadOnlyList<Column>> select,
        TTable table,
        Func<TTable, Conditions> where
    ) {
        Name = TextFieldSm.From(Guid.NewGuid().ToString());
        FrequencyPerMonth = Positive<double>.Zero;
        Selectivity = Positive<int>.Zero;
        Select = select;
        Table = table;
        Where = where;

        WhereConditions = Where(table);

        SelectColumns = Select(table)
            .Distinct()
            .ToIReadOnlyList();
        WhereColumns = WhereConditions.DistinctColumns();
        AllColumns = SelectColumns
            .Concat(WhereColumns)
            .ToIReadOnlyList();

        Sql = new SqlQueryBuilder()
            .Select(SelectColumns)
            .From(table)
            .Where(WhereConditions)
            .ToSql();
    }
}


public sealed record Application<TTable1, TTable2> : IApplication
    where TTable1 : Table<TTable1>
    where TTable2 : Table<TTable2>
{
    public required TextFieldSm Name { get; init; }
    public required Positive<double> FrequencyPerMonth { get; init; }
    public required Positive<int> Selectivity { get; init; }
    public required Func<TTable1, TTable2, IReadOnlyList<Column>> Select { get; init; }
    public required AliasedTable<TTable1> Table1 { get; init; }
    public required AliasedTable<TTable2> Table2 { get; init; }
    public required Func<TTable1, TTable2, Conditions> On { get; init; }
    public required Func<TTable1, TTable2, Conditions> Where { get; init; }
    public required Conditions WhereConditions { get; init; }
    public required IReadOnlyList<Column> SelectColumns { get; init; }
    public required IReadOnlyList<Column> WhereColumns { get; init; }
    public required IReadOnlyList<Column> OnColumns { get; init; }
    public required IReadOnlyList<Column> AllColumns { get; init; }
    public required string Sql { get; init; }

    public Application<TTable1, TTable2> WithFrequencyPerMonth(double frequencyPerMonth) =>
        WithFrequencyPerMonth(Positive<double>.From(frequencyPerMonth));

    public Application<TTable1, TTable2> WithFrequencyPerMonth(Positive<double> frequencyPerMonth) =>
        this with { FrequencyPerMonth = frequencyPerMonth, };

    public Application<TTable1, TTable2> WithSelectivity(int selectivity) =>
        WithSelectivity(Positive<int>.From(selectivity));

    public Application<TTable1, TTable2> WithSelectivity(Positive<int> selectivity) =>
        this with { Selectivity = selectivity, };

    public Application<TTable1, TTable2> WithName(string name) =>
        WithName(TextFieldSm.From(name));

    public Application<TTable1, TTable2> WithName(TextFieldSm name) =>
        this with { Name = name, };

    public static Application<TTable1, TTable2> New(
        Func<TTable1, TTable2, IReadOnlyList<Column>> select,
        AliasedTable<TTable1> table1,
        AliasedTable<TTable2> table2,
        Func<TTable1, TTable2, Conditions> on,
        Func<TTable1, TTable2, Conditions> where
    ) => new(select, table1, table2, on, where);

    [SetsRequiredMembers]
    private Application(
        Func<TTable1, TTable2, IReadOnlyList<Column>> select,
        AliasedTable<TTable1> table1,
        AliasedTable<TTable2> table2,
        Func<TTable1, TTable2, Conditions> on,
        Func<TTable1, TTable2, Conditions> where
    ) {
        Name = TextFieldSm.From(Guid.NewGuid().ToString());
        FrequencyPerMonth = Positive<double>.Zero;
        Selectivity = Positive<int>.Zero;
        Select = select;
        Table1 = table1;
        Table2 = table2;
        On = on;
        Where = where;

        Conditions onConditions = On(table1, table2);
        WhereConditions = Where(table1, table2);

        SelectColumns = Select(table1, table2)
            .ToIReadOnlyList();
        OnColumns = onConditions.DistinctColumns();
        WhereColumns = WhereConditions.DistinctColumns();
        AllColumns = SelectColumns
            .Concat(OnColumns)
            .Concat(WhereColumns)
            .ToIReadOnlyList();

        Sql = new SqlQueryBuilder()
            .Select(SelectColumns)
            .From(table1)
            .InnerJoin(table2)
            .On(onConditions)
            .Where(WhereConditions)
            .ToSql();
    }
}


public class SqlQueryBuilder
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

    public SqlQueryBuilder On(Conditions onConditions)
    {
        _stringBuilder.AppendLine($"\tON {string.Join("\n\tAND ", onConditions.Select(c => c.Value.Sql))} ");
        return this;
    }

    public SqlQueryBuilder Where(Conditions whereConditions)
    {
        _stringBuilder.AppendLine($"WHERE {string.Join("\n\tAND ", whereConditions.Select(c => c.Value.Sql))}");
        return this;
    }

    public string ToSql() => _stringBuilder
        .ToString()
        .TrimEnd();
}
