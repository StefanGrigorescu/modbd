using Dapper;
using MODBD_Api.Common;
using MODBD_Api.Common.Contracts;
using MODBD_Common.Abstractions.Responses;
using MODBD_Common.Collections;
using System.Data;
using System.Diagnostics;

namespace MODBD_Api.Sales;

public sealed record SelectOrdersWithItemsSpecification
{
    public required Tenant Tenant { get; init; }
    private string Orders => Tenant == Tenant.Global ?
        "vw_sls_orders" :
        "sls_orders";
    private string OrderItems => Tenant == Tenant.Global ?
        "vw_sls_order_items" :
        "sls_order_items";
    private string Products => Tenant == Tenant.Global ?
        "vw_sls_products" :
        "sls_products";
    private string OrderStatuses => Tenant == Tenant.Global ?
        "vw_sls_order_statuses" :
        "sls_order_statuses";

    private string _where = string.Empty;

    private SelectOrdersWithItemsSpecification() { }

    public static SelectOrdersWithItemsSpecification FromTenant(Tenant tenant) => new() { Tenant = tenant };

    public SelectOrdersWithItemsSpecification Where(string condition) =>
        this with { _where = condition };

    public async Task<IEnumerable<OrderResponse>> QueryAsync(
        IDbConnection db,
        RowOffset rowOffset, RowCount rowCount,
        DynamicParameters parameters
    ) => (await db.QueryAsync<OrderWithItemDb>(AsQuery(rowOffset, rowCount), parameters))
            .GroupBy((OrderWithItemDb owi) => owi.Id)
            .Select(MapToOrderResponse);

    public async Task<IEnumerable<OrderResponse>> QueryAsync(
        IDbConnection db,
        RowOffset rowOffset, RowCount rowCount
    ) => (await db.QueryAsync<OrderWithItemDb>(AsQuery(rowOffset, rowCount)))
            .GroupBy((OrderWithItemDb owi) => owi.Id)
            .Select(MapToOrderResponse);

    private string AsQuery(RowOffset rowOffset, RowCount rowCount)
    {
        string whereCondition = string.IsNullOrWhiteSpace(_where) ?
            string.Empty :
            $"WHERE {_where}";

        return $@"
            WITH PaginatedOrders AS (
                SELECT 
                    o.id AS Id, 
                    o.customer_id AS CustomerId, 
                    o.customer_region_id AS CustomerRegionId, 
                    o.address AS Address, 
                    o.created_on AS CreatedOn, 
                    o.last_updated_on AS LastUpdatedOn, 
                    o.status_id AS StatusId
                FROM {Orders} o
                {whereCondition}
                ORDER BY o.created_on DESC
                OFFSET {rowOffset.Value} ROWS FETCH NEXT {rowCount.Value} ROWS ONLY
            )
            SELECT 
                po.Id, 
                po.CustomerId, 
                po.CustomerRegionId, 
                po.Address, 
                po.CreatedOn, 
                po.LastUpdatedOn, 
                
                os.name AS Status, 
                
                oi.quantity AS Quantity, 
                
                p.id AS ProductId , 
                p.name AS ProductName, 
                p.description AS ProductDescription, 
                p.price_in_eur AS ProductPriceInEur, 
                p.created_on AS ProductCreatedOn, 
                p.last_updated_on AS ProductLastUpdatedOn 

            FROM PaginatedOrders po
            INNER JOIN {OrderStatuses} os
                ON po.StatusId = os.id
            LEFT JOIN {OrderItems} oi 
                ON po.Id = oi.order_id
            LEFT JOIN {Products} p 
                ON oi.product_id = p.id 
        ";
    }

    private static OrderResponse MapToOrderResponse(IGrouping<long, OrderWithItemDb> orderItems)
    {
        long id = orderItems.Key;
        OrderWithItemDb order = orderItems.First();
        OrderWithItemDb[] orderItemsArray = [.. orderItems];
        return new()
        {
            Id = id,
            Tenant = Tenant.FromRegionId(order.CustomerRegionId),
            CustomerId = order.CustomerId,
            CustomerRegionId = order.CustomerRegionId,
            Address = order.Address,
            Status = order.Status,
            CreatedOn = order.CreatedOn,
            LastUpdatedOn = order.LastUpdatedOn,
            Items = orderItemsArray
                .Select(MapToOrderItemResponse)
                .Where(response => response.IsSuccess)
                .ToIReadOnlyList(response => 
                    response.TryGetData(out ProductResponse? product) ?
                        product :
                        throw new UnreachableException("Response succeeded was expected. Failed was found instead.")
                ),
            TotalPriceInEur = orderItemsArray.Sum(oi => (oi?.ProductPriceInEur ?? 0) * (oi?.Quantity ?? 0)),
        };
    }

    private static AppResponse<ProductResponse> MapToOrderItemResponse(OrderWithItemDb orderItem) => orderItem.ProductId is null ?
        AppResponse<ProductResponse>.Failed("No product found.") :
        AppResponse<ProductResponse>.Succeeded(new ()
        {
            Id = orderItem.ProductId!.Value,
            Name = orderItem.ProductName!,
            Description = orderItem.ProductDescription!,
            PriceInEur = orderItem.ProductPriceInEur!.Value,
            CreatedOn = orderItem.ProductCreatedOn!.Value,
            LastUpdatedOn = orderItem.ProductLastUpdatedOn,
        });

    private sealed record OrderWithItemDb
    {
        public required long Id { get; init; }
        public required long CustomerId { get; init; }
        public required int CustomerRegionId { get; init; }
        public required string Address { get; init; }
        public required DateTime CreatedOn { get; init; }
        public required DateTime LastUpdatedOn { get; init; }
        public required string Status { get; init; }
        public required int? Quantity { get; init; }
        public required long? ProductId { get; init; }
        public required string? ProductName { get; init; }
        public required string? ProductDescription { get; init; }
        public required decimal? ProductPriceInEur { get; init; }
        public required DateTime? ProductCreatedOn { get; init; }
        public required DateTime? ProductLastUpdatedOn { get; init; }
    }
}
