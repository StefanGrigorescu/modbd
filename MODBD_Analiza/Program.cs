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

Conditions completeMinimalPredicates = CompleteMinimalPredicates.Of(ordersApplications, output);
output.WriteLine("Complete minimal predicates of orders: {");
foreach (ICondition condition in completeMinimalPredicates.Values)
{
    output.WriteLine(condition.Sql);
}
output.WriteLine("}\n");

output.WriteLine("Finished running program.");
