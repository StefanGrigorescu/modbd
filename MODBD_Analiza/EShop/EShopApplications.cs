using MODBD_Analiza.Applications;

namespace MODBD_Analiza.EShop;

internal sealed record IdntUsersApplications : EntityApplications<IdntUsers>
{
    public required Application<IdntUsers> LoginWithEmail { get; init; }
    public required Application<IdntUsers> GetDetailsAfterLoginWithEmail { get; init; }
    public required Application<IdntUsers> LoginWithUsername { get; init; }
    public required Application<IdntUsers> GetDetailsAfterLoginWithUsername { get; init; }

    public static IdntUsersApplications New(EShopSchema eshop)
    {
        IdntUsersApplications apps = new()
        {
            LoginWithEmail = eshop
            .IdntUsers
            .Select(u => [u.Id, u.Salt, u.Password])
            .Where(u => u.Email.Equal(new SqlQueryParameter("p_email"))),

            GetDetailsAfterLoginWithEmail = eshop
            .IdntUsers
            .Select(u => [u.Username, u.FirstName, u.LastName, u.DateOfBirth, u.PhoneNumber, u.RegionId, u.CreatedOn, u.LastUpdatedOn])
            .Where(u => u.Id.Equal(new SqlQueryParameter("p_id"))),

            LoginWithUsername = eshop
            .IdntUsers
            .Select(u => [u.Id, u.Salt, u.Password])
            .Where(u => u.Username.Equal(new SqlQueryParameter("p_username"))),

            GetDetailsAfterLoginWithUsername = eshop
            .IdntUsers
            .Select(u => [u.Email, u.FirstName, u.LastName, u.DateOfBirth, u.PhoneNumber, u.RegionId, u.CreatedOn, u.LastUpdatedOn])
            .Where(u => u.Id.Equal(new SqlQueryParameter("p_id"))),

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

    private IdntUsersApplications() { }
}


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


internal sealed record SlsOrderItemsApplications : EntityApplications<SlsOrderItems>
{
    public required Application<SlsOrderItems, SlsOrders> GetOrderItemsInRegion { get; init; }

    public static SlsOrderItemsApplications New(EShopSchema eshop)
    {
        SlsOrderItemsApplications apps = new()
        {
            GetOrderItemsInRegion = eshop
            .SlsOrderItems.As("oi")
            .InnerJoin(
                eshop.SlsOrders.As("o"),
                (oi, o) => oi.OrderId.Equal(o.Id)
            ).Select((oi, o) => [oi.OrderId, oi.ProductId, oi.Quantity, oi.CreatedOn, oi.LastUpdatedOn])
            .Where((oi, o) => o.CustomerRegionId.Equal(new SqlQueryParameter("p_region_id"))),

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

    private SlsOrderItemsApplications() { }
}
