namespace MODBD_Analiza.Schema;

internal record Column
{
    public required string Name { get; init; }

    public virtual string ToSql() => Name;

    //public required string Type { get; init; }
    //public required bool IsPrimaryKey { get; init; }
    //public required bool IsForeignKey { get; init }
    //public required string ForeignKeyTable { get; init; }
    //public required string ForeignKeyColumn { get; init; }
}


internal sealed record ColumnOfTable : Column
{
    public required string TableAlias { get; init; }

    public override string ToSql() => $"{TableAlias}.{Name}";

    public static ColumnOfTable New(string tableAlias, string columnName) => new() 
    { 
        TableAlias = tableAlias, 
        Name = columnName, 
    };

    public static ColumnOfTable New(string tableAlias, Column column) => new() { 
        TableAlias = tableAlias, 
        Name = column.Name, 
    };

    private ColumnOfTable() { }
}
