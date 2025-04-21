using MODBD_Analiza;
using MODBD_Analiza.EShop;

Output output = new();

output.WriteLine("Hello, World!\n");

EShopSchema schema = EShopSchema.New();
EShopApplications applications = EShopApplications.New(schema);

foreach(string sql in applications.All.Select(a => a.Sql))
{
    output.WriteLine($"{sql}\n");
}

output.WriteLine("Finished running program.");
