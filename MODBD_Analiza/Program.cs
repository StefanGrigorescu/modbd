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
string slsOrdersTableName = typeof(SlsOrders).Name;
output.Write($"Horizontal shards of {slsOrdersTableName}: ");
output.WriteLine("{");
for (int i = 0; i < ordersHorizontalShards.Count; i++)
{
    Conditions ordersHorizontalShard = ordersHorizontalShards[i];
    output.WriteLine($"\t{slsOrdersTableName}{i} =  {ordersHorizontalShard.ToSql()}");
}
output.WriteLine("}\n");

string slsOrderItemsTableName = typeof(SlsOrderItems).Name;
output.Write($"Horizontal shards of {slsOrderItemsTableName}: ");
output.WriteLine("{");
for (int i = 0; i < ordersHorizontalShards.Count; i++)
{
    output.WriteLine($"\t{slsOrderItemsTableName}{i} =  {slsOrderItemsTableName} of {slsOrdersTableName}{i}");
}
output.WriteLine("}\n");



output.WriteLine("Finished running program.");
