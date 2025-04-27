using MODBD_Core.Applications;
using static MODBD_Core.Applications.QuerySingleTable;

namespace MODBD_Core.EShop;

public sealed record SlsOrdersApplications : EntityApplications<SlsOrders>
{
    public required Application<SlsOrders> GetOrdersInMuntenia { get; init; }
    public required Application<SlsOrders> GetOrdersNotInMuntenia { get; init; }
    public required Application<SlsOrders> GetCompletedOrders { get; init; }
    public required Application<SlsOrders> GetNotCompletedOrders { get; init; }
    public required Application<SlsOrders> GetCanceledOrders { get; init; }
    public required Application<SlsOrders> GetNotCanceledOrders { get; init; }

    public required Application<SlsOrders> GetCompletedOrdersInMuntenia { get; init; }
    public required Application<SlsOrders> GetNotCompletedOrdersInMuntenia { get; init; }
    public required Application<SlsOrders> GetCanceledOrdersInMuntenia { get; init; }
    public required Application<SlsOrders> GetNotCanceledOrdersInMuntenia { get; init; }

    public required Application<SlsOrders> GetCompletedOrdersNotInMuntenia { get; init; }
    public required Application<SlsOrders> GetNotCompletedOrdersNotInMuntenia { get; init; }
    public required Application<SlsOrders> GetCanceledOrdersNotInMuntenia { get; init; }
    public required Application<SlsOrders> GetNotCanceledOrdersNotInMuntenia { get; init; }


    private const int _idMuntenia = 0;
    private const int _idCompleted = 1;
    private const int _idCanceled = 2;

    
    public static SlsOrdersApplications New(EShopSchema eshop)
    {
        QueryBuilder<SlsOrders> selectFromSlsOrders() => eshop
            .SlsOrders
            .Select(o => [o.Id, o.CustomerId, o.Address, o.StatusId, o.CreatedOn, o.LastUpdatedOn]);

        SlsOrdersApplications apps = new()
        {
            GetOrdersInMuntenia = selectFromSlsOrders()
                .Where(o => o.CustomerRegionId.Equal(_idMuntenia))
                .WithFrequencyPerMonth(30000)
                .WithSelectivity(600_000_000)
                .WithName(nameof(GetOrdersInMuntenia))
                .WithIsMain(),

            GetOrdersNotInMuntenia = selectFromSlsOrders()
                .Where(o => o.CustomerRegionId.NotEqual(_idMuntenia))
                .WithFrequencyPerMonth(3000)
                .WithSelectivity(300_000_000)
                .WithName(nameof(GetOrdersNotInMuntenia)),

            GetCompletedOrders = selectFromSlsOrders()
                .Where(o => o.StatusId.Equal(_idCompleted))
                .WithFrequencyPerMonth(2000)
                .WithSelectivity(650_000_000)
                .WithName(nameof(GetCompletedOrders))
                .WithIsMain(),

            GetNotCompletedOrders = selectFromSlsOrders()
                .Where(o => o.StatusId.NotEqual(_idCompleted))
                .WithFrequencyPerMonth(100)
                .WithSelectivity(250_000_000)
                .WithName(nameof(GetNotCompletedOrders)),

            GetCanceledOrders = selectFromSlsOrders()
                .Where(o => o.StatusId.Equal(_idCanceled))
                .WithFrequencyPerMonth(150)
                .WithSelectivity(200_000_000)
                .WithName(nameof(GetCanceledOrders)),

            GetNotCanceledOrders = selectFromSlsOrders()
                .Where(o => o.StatusId.NotEqual(_idCanceled))
                .WithFrequencyPerMonth(60)
                .WithSelectivity(700_000_000)
                .WithName(nameof(GetNotCanceledOrders))
                .WithIsMain(),


            GetCompletedOrdersInMuntenia = selectFromSlsOrders()
                .Where(o =>
                    o.StatusId.Equal(_idCompleted)
                    .And(o.CustomerRegionId.Equal(_idMuntenia))
                ).WithFrequencyPerMonth(900)
                .WithSelectivity(550_000_000)
                .WithName(nameof(GetCompletedOrdersInMuntenia))
                .WithIsMain(),

            GetNotCompletedOrdersInMuntenia = selectFromSlsOrders()
                .Where(o =>
                    o.StatusId.NotEqual(_idCompleted)
                    .And(o.CustomerRegionId.Equal(_idMuntenia))
                ).WithFrequencyPerMonth(300)
                .WithSelectivity(50_000_000)
                .WithName(nameof(GetNotCompletedOrdersInMuntenia)),

            GetCanceledOrdersInMuntenia = selectFromSlsOrders()
                .Where(o =>
                    o.StatusId.Equal(_idCanceled)
                    .And(o.CustomerRegionId.Equal(_idMuntenia))
                ).WithFrequencyPerMonth(60)
                .WithSelectivity(35_000_000)
                .WithName(nameof(GetCanceledOrdersInMuntenia)),

            GetNotCanceledOrdersInMuntenia = selectFromSlsOrders()
                .Where(o =>
                    o.StatusId.NotEqual(_idCanceled)
                    .And(o.CustomerRegionId.Equal(_idMuntenia))
                ).WithFrequencyPerMonth(3)
                .WithSelectivity(565_000_000)
                .WithName(nameof(GetNotCanceledOrdersInMuntenia))
                .WithIsMain(),


            GetCompletedOrdersNotInMuntenia = selectFromSlsOrders()
                .Where(o =>
                    o.StatusId.Equal(_idCompleted)
                    .And(o.CustomerRegionId.NotEqual(_idMuntenia))
                ).WithFrequencyPerMonth(7)
                .WithSelectivity(235_000_000)
                .WithName(nameof(GetCompletedOrdersNotInMuntenia)),

            GetNotCompletedOrdersNotInMuntenia = selectFromSlsOrders()
                .Where(o =>
                    o.StatusId.NotEqual(_idCompleted)
                    .And(o.CustomerRegionId.NotEqual(_idMuntenia))
                ).WithFrequencyPerMonth(0.5)
                .WithSelectivity(65_000_000)
                .WithName(nameof(GetNotCompletedOrdersNotInMuntenia)),

            GetCanceledOrdersNotInMuntenia = selectFromSlsOrders()
                .Where(o =>
                    o.StatusId.Equal(_idCanceled)
                    .And(o.CustomerRegionId.NotEqual(_idMuntenia))
                ).WithFrequencyPerMonth(1.5)
                .WithSelectivity(10_000_000)
                .WithName(nameof(GetCanceledOrdersNotInMuntenia)),

            GetNotCanceledOrdersNotInMuntenia = selectFromSlsOrders()
                .Where(o =>
                    o.StatusId.NotEqual(_idCanceled)
                    .And(o.CustomerRegionId.NotEqual(_idMuntenia))
                ).WithFrequencyPerMonth(0.125)
                .WithSelectivity(225_000_000)
                .WithName(nameof(GetNotCanceledOrdersNotInMuntenia)),


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

    private SlsOrdersApplications() { }
}
