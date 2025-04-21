using MODBD_Analiza.Applications;
using MODBD_Analiza.Schema;

namespace MODBD_Analiza.IO;

internal static class WriteApplications
{
    public static void WriteEntityApplications<TTable>(this Output output, EntityApplications<TTable> applications)
        where TTable : Table<TTable>
    {
        output.Write($"{typeof(TTable).Name} applications = ");
        output.WriteLine("{\n");

        foreach (string sql in applications.All.Select(a => a.Sql))
        {
            output.WriteLine($"{sql}\n");
        }

        output.WriteLine("}\n");
    }
}
