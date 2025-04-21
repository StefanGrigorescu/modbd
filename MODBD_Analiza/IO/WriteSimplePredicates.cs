using MODBD_Analiza.Applications;
using MODBD_Analiza.Schema;

namespace MODBD_Analiza.IO;

internal static class WriteSimplePredicates
{
    public static void WriteSimplePredicatesOf<TTable>(this Output output, EntityApplications<TTable> applications)
        where TTable : Table<TTable>
    {
        output.Write($"Simple predicates of {typeof(TTable).Name} = ");
        output.WriteLine("{"); 
        
        output.WriteLine(string.Join(",\n", applications.AllSimplePredicates.Select(p => $"\t{p.Sql}")));

        output.WriteLine("}\n");
    }
}
