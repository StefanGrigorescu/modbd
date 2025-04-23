using MODBD_Core.Schema;

namespace MODBD_Core.Applications;

public static class QuerySingleTable
{
    public static QueryBuilder<TTable> Select<TTable>(
        this TTable table,
        Func<TTable, IReadOnlyList<Column>> select
    ) where TTable : Table<TTable>
        => new(table, select);


    public sealed record QueryBuilder<TTable>(
        TTable Table,
        Func<TTable, IReadOnlyList<Column>> Select
    ) where TTable : Table<TTable>
    {
        public Application<TTable> Where(Func<TTable, IReadOnlyList<ConditionWithValue>> where) => Application<TTable>.New(Select, Table, where);
    }
}


public static class QueryTwoTables
{
    public static JoinBuilder<TTable1, TTable2> InnerJoin<TTable1, TTable2>(
        this AliasedTable<TTable1> table1,
        AliasedTable<TTable2> table2,
        Func<TTable1, TTable2, IReadOnlyList<ConditionWithOtherColumn>> on
    )
        where TTable1 : Table<TTable1>
        where TTable2 : Table<TTable2>
     => new(table1, table2, on);


    public sealed class JoinBuilder<TTable1, TTable2>(
        AliasedTable<TTable1> Table1,
        AliasedTable<TTable2> Table2,
        Func<TTable1, TTable2, IReadOnlyList<ConditionWithOtherColumn>> On
    )
        where TTable1 : Table<TTable1>
        where TTable2 : Table<TTable2>
    {
        public QueryBuilder<TTable1, TTable2> Select(Func<TTable1, TTable2, IReadOnlyList<Column>> select) =>
            new(Table1, Table2, On, select);
    }


    public sealed class QueryBuilder<TTable1, TTable2>(
        AliasedTable<TTable1> Table1,
        AliasedTable<TTable2> Table2,
        Func<TTable1, TTable2, IReadOnlyList<ConditionWithOtherColumn>> On,
        Func<TTable1, TTable2, IReadOnlyList<Column>> Select
    )
        where TTable1 : Table<TTable1>
        where TTable2 : Table<TTable2>
    {
        public Application<TTable1, TTable2> Where(
            Func<TTable1, TTable2, IReadOnlyList<ICondition>> where
        ) => Application<TTable1, TTable2>.New(Select, Table1, Table2, On, where);
    }
}
