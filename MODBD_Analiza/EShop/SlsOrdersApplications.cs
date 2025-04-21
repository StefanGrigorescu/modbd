using MODBD_Analiza.Applications;

namespace MODBD_Analiza.EShop;

internal sealed record SlsOrdersApplications : EntityApplications<SlsOrders>
{
    public required Application<SlsOrders> GetOrdersInRegion { get; init; }
    public required Application<SlsOrders> GetCompletedOrders { get; init; }
    public required Application<SlsOrders> GetCanceledOrders { get; init; }

    public static SlsOrdersApplications New(EShopSchema eshop)
    {
        SlsOrdersApplications apps = new()
        {
            GetOrdersInRegion = eshop
            .SlsOrders
            .Select(o => [o.Id, o.CustomerId, o.Address, o.StatusId, o.CreatedOn, o.LastUpdatedOn])
            .Where(o => o.CustomerRegionId.Equal(new SqlQueryParameter("p_region_id"))),

            GetCompletedOrders = eshop
            .SlsOrders
            .Select(o => [o.Id, o.CustomerId, o.CustomerRegionId, o.Address, o.CreatedOn, o.LastUpdatedOn])
            .Where(o => o.StatusId.Equal(1)),

            GetCanceledOrders = eshop
            .SlsOrders
            .Select(o => [o.Id, o.CustomerId, o.CustomerRegionId, o.Address, o.CreatedOn, o.LastUpdatedOn])
            .Where(o => o.StatusId.Equal(2)),

            All = [],
            AllSimplePredicates = [],
        };

        IReadOnlyList<IApplication> all = apps.GetAllApplications();
        IReadOnlyList<ICondition> allSimplePredicates = all.GetAllSimplePredicates();

        return apps with
        {
            All = all,
            AllSimplePredicates = allSimplePredicates,
        };
    }

    private SlsOrdersApplications() { }
}
