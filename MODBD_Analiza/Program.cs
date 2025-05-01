using MODBD_Core.Applications;
using MODBD_Core.Applications.SqlConditions;
using MODBD_Core.EShop;
using MODBD_Core.IO;

ConsoleOutput output = new();
//FileOutput output = new("analiza.txt");

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

IReadOnlyList<Conditions> ordersHorizontalShards = PrimaryHorizontalSharding.Of(ordersApplications, output);
output.Write($"Horizontal shards of {typeof(SlsOrders).Name}: ");
output.WriteLine("{");
foreach (Conditions ordersHorizontalShard in ordersHorizontalShards)
{
    output.WriteLine($"\t{ordersHorizontalShard.ToSql()}");
}
output.WriteLine("}\n");

output.WriteLine("Finished running program.");
