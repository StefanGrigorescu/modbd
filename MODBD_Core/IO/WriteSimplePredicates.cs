using MODBD_Core.Applications;
using MODBD_Core.Schema;

namespace MODBD_Core.IO;

public static class WriteSimplePredicates
{
    public static void WriteSimplePredicatesOf<TTable>(this IOutput output, EntityApplications<TTable> applications)
        where TTable : Table<TTable>
    {
        output.Write($"Simple predicates of {typeof(TTable).Name} = ");
        output.WriteLine("{"); 
        
        output.WriteLine(string.Join(",\n", applications.AllSimplePredicates.Select(p => $"\t{p.Sql}")));

        output.WriteLine("}\n");
    }
}
