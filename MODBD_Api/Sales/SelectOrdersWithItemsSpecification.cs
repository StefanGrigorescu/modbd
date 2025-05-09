using Dapper;
using MODBD_Api.Common;
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

    private string AsQuery()
    {
        string whereCondition = string.IsNullOrWhiteSpace(_where) ? 
            string.Empty : 
            $"WHERE {_where}";

        return $@"
            SELECT 
                o.id, 
                o.customer_id, 
                o.customer_region_id, 
                o.address, 
                o.created_on, 
                o.last_updated_on,
            
                os.name AS status, 

                oi.quantity,

                p.id AS product_id, 
                p.name AS product_name, 
                p.description AS product_description, 
                p.price_in_eur AS product_price_in_eur, 
                p.created_on AS product_created_on, 
                p.last_updated_on AS product_last_updated_on 

            FROM {Orders} o
            INNER JOIN {OrderStatuses} os
                ON o.status_id = os.id
            LEFT JOIN {OrderItems} oi 
                ON o.id = oi.order_id
            LEFT JOIN {Products} p 
                ON oi.product_id = p.id

            {whereCondition}
        ";
    }

    private string _where = string.Empty;

    private SelectOrdersWithItemsSpecification() { }

    public static SelectOrdersWithItemsSpecification FromTenant(Tenant tenant) => new() { Tenant = tenant };

    public SelectOrdersWithItemsSpecification Where(string condition) =>
        this with { _where = condition };

    public async Task<IEnumerable<OrderResponse>> QueryAsync(
        IDbConnection db,
        DynamicParameters parameters
    ) => (await db.QueryAsync<OrderWithItemDb>(AsQuery(), parameters))
            .GroupBy((OrderWithItemDb owi) => owi.Id)
            .Select(MapToOrderResponse);

    public async Task<IEnumerable<OrderResponse>> QueryAsync(
        IDbConnection db
    ) => (await db.QueryAsync<OrderWithItemDb>(AsQuery()))
            .GroupBy((OrderWithItemDb owi) => owi.Id)
            .Select(MapToOrderResponse);

    private static OrderResponse MapToOrderResponse(IGrouping<long, OrderWithItemDb> orderItems)
    {
        long id = orderItems.Key;
        OrderWithItemDb order = orderItems.First();
        OrderWithItemDb[] orderItemsArray = [.. orderItems];
        return new()
        {
            Id = id,
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
