using MODBD_Analiza;

Output output = new();

output.WriteLine("Hello, World!");

EShopSchema schema = EShopSchema.New();
EShopApplications applications = EShopApplications.All(schema);
