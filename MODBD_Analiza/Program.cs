using MODBD_Analiza.EShop;
using MODBD_Analiza.IO;

Output output = new();

output.WriteLine("Hello, World!\n");

EShopSchema schema = EShopSchema.New();

IdntUsersApplications usersApplications = IdntUsersApplications.New(schema);
SlsOrdersApplications ordersApplications = SlsOrdersApplications.New(schema);
SlsOrderItemsApplications orderItemsApplications = SlsOrderItemsApplications.New(schema);

output.WriteEntityApplications(usersApplications);
output.WriteEntityApplications(ordersApplications);
output.WriteEntityApplications(orderItemsApplications);

output.WriteSimplePredicatesOf(usersApplications);
output.WriteSimplePredicatesOf(ordersApplications);
output.WriteSimplePredicatesOf(orderItemsApplications);

output.WriteLine("Finished running program.");
