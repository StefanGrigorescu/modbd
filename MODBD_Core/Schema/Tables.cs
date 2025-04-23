using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace MODBD_Core.Schema;

public interface ITable
{
    string Name { get; }
    string ToSql();
}


public abstract record Table<TTable> : ITable
    where TTable : Table<TTable>
{
    public abstract string Name { get; init; }
    public abstract IReadOnlyList<Column> AllColumns { get; init; }

    public virtual string ToSql() => Name.ToLowerInvariant();

    public abstract AliasedTable<TTable> As(string alias);

    public abstract TTable Copy();

    public IReadOnlyList<Column> GetAllColumns() =>
        GetType()
            .GetProperties()
            .Where(p => p.PropertyType.IsAssignableTo(typeof(Column)))
            .Select(p => (p.GetValue(this) as Column)!)
            .ToArray()
            .AsReadOnly();
}


public sealed record AliasedTable<TTable> : ITable
    where TTable : Table<TTable>
{
    public required TTable Table { get; init; }
    public required string Alias { get; init; }

    public string Name => Table.Name;
    public static implicit operator TTable(AliasedTable<TTable> table) => table.Table;

    public string ToSql() => $"{Name.ToLowerInvariant()} {Alias}";

    public static AliasedTable<TTable> New(TTable table, string alias) =>
        new(table, alias) { };

    [SetsRequiredMembers]
    private AliasedTable(TTable table, string alias)
    {
        Table = PrefixTableColumns(table, alias);
        Alias = alias;
    }

    private static TTable PrefixTableColumns(TTable table, string alias)
    {
        TTable newTable = table.Copy();

        IEnumerable<PropertyInfo> properties = typeof(TTable)
            .GetProperties()
            .Where(p => p.PropertyType.IsAssignableTo(typeof(Column)));

        foreach (PropertyInfo property in properties)
        {
            Column originalColumn = (Column)property.GetValue(table)!;

            property.SetValue(
                newTable,
                ColumnOfTable.New(alias, originalColumn)
            );
        }

        return newTable;
    }
}
