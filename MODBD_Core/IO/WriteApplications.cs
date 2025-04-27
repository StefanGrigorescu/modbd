using MODBD_Core.Applications;
using MODBD_Core.Schema;

namespace MODBD_Core.IO;

public static class WriteApplications
{
    public static void WriteEntityApplications<TTable>(this IOutput output, EntityApplications<TTable> applications)
        where TTable : Table<TTable>
    {
        output.Write($"{typeof(TTable).Name} applications = ");
        output.WriteLine("{\n");

        foreach (IApplication app in applications.AllMain)
        {
            output.WriteLine($"{app.Name}:");
            output.WriteLine($"{app.Sql};");
            output.WriteLine($"frequency per month = {app.FrequencyPerMonth} | selectivity = {app.Selectivity}\n");
        }

        output.WriteLine("}\n");
    }
}
