using MODBD_Core.Applications;
using MODBD_Core.Applications.SqlConditions;

namespace MODBD_Core.EShop;

public sealed record SlsOrderItemsApplications : EntityApplications<SlsOrderItems>
{
    //public required Application<SlsOrderItems, SlsOrders> GetOrderItemsInRegion { get; init; }

    public static SlsOrderItemsApplications New(EShopSchema eshop)
    {
        SlsOrderItemsApplications apps = new()
        {
            //GetOrderItemsInRegion = eshop
            //    .SlsOrderItems.As("oi")
            //    .InnerJoin(
            //        eshop.SlsOrders.As("o"),
            //        (oi, o) => oi.OrderId.Equal(o.Id)
            //    ).Select((oi, o) => [oi.OrderId, oi.ProductId, oi.Quantity, oi.CreatedOn, oi.LastUpdatedOn])
            //    .Where((oi, o) => o.CustomerRegionId.Equal(new SqlQueryParameter("p_region_id")))
            //    .WithName(nameof(GetOrderItemsInRegion)),

            All = [],
            AllSimplePredicates = [],
            AllMain = [],
        };

        IReadOnlyList<IApplication> all = apps.GetAllApplications();
        IReadOnlyList<ICondition> allSimplePredicates = all.GetAllSimplePredicates();
        IReadOnlyList<IApplication> allMainApplications = all.GetAllMainApplications();

        return apps with
        {
            All = all,
            AllSimplePredicates = allSimplePredicates,
            AllMain = allMainApplications,
        };
    }

    private SlsOrderItemsApplications() { }
}
