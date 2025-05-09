using Dapper;
using Microsoft.AspNetCore.Mvc;
using MODBD_Api.Common;
using MODBD_Api.Common.Contracts;
using MODBD_Api.Common.Persistence;
using MODBD_Common.Abstractions;
using MODBD_Common.Abstractions.Responses;
using System.Data;

namespace MODBD_Api.Sales.AppQueries;

// ======= Response DTO =======
public sealed record GetOrderResponse
{
    public required decimal Id            { get; init; }
    public required int      CustomerId    { get; init; }
    public required DateTime OrderDate     { get; init; }
    public required decimal  TotalAmount   { get; init; }
    public required string   Status        { get; init; }
    public required DateTime CreatedOn     { get; init; }
    public required DateTime LastUpdatedOn { get; init; }
}
public sealed record PlaceOrderRequest
{
    public required int CustomerId { get; init; }
    public required string Address { get; init; }
    public required string ItemsCsv { get; init; }
}

// ======= Queries =======
public sealed record GetOrdersQuery : IRequest<IEnumerable<GetOrderResponse>>
{
    public required int TenantId { get; init; }
}
public sealed record GetOrderQuery(int Id) : IRequest<GetOrderResponse>
{
    public required int TenantId { get; init; }
}
public sealed record GetMyOrdersQuery(int CustomerId) : IRequest<IEnumerable<GetOrderResponse>>
{
    public required int TenantId { get; init; }
}
public sealed record PlaceOrderCommand(PlaceOrderRequest Request, int TenantId) : IRequest<bool>;

// ======= Controllers =======
[ApiController]
public sealed class GetOrdersController : ControllerBase
{
    private readonly GetOrdersQueryHandler _handler;

    public GetOrdersController(GetOrdersQueryHandler handler)
    {
        _handler = handler;
    }

    [HttpGet(ApiRoutes.Sales.GetOrders, Name = "get-orders")]
    [Tags(ApiRoutes.Sales.Tag)]
    public async Task<IActionResult> GetOrders([FromRoute] int tenantId, CancellationToken cancellationToken = default)
    {
        GetOrdersQuery query = new() { TenantId = tenantId };
        AppResponse<IEnumerable<GetOrderResponse>> response = await _handler.Handle(query, cancellationToken);
        return this.From(response);
    }
}

public sealed class GetOrderController : ControllerBase
{
    private readonly GetOrderQueryHandler _handler;

    public GetOrderController(GetOrderQueryHandler handler)
    {
        _handler = handler;
    }

    [HttpGet(ApiRoutes.Sales.GetOrder, Name = "get-order")]
    [Tags(ApiRoutes.Sales.Tag)]
    public async Task<IActionResult> GetOrder([FromRoute] int tenantId, [FromRoute] int id, CancellationToken cancellationToken = default)
    {
        GetOrderQuery query = new(id) { TenantId = tenantId };
        AppResponse<GetOrderResponse> response = await _handler.Handle(query, cancellationToken);
        return this.From(response);
    }
}

public sealed class GetMyOrdersController : ControllerBase
{
    private readonly GetMyOrdersQueryHandler _handler;

    public GetMyOrdersController(GetMyOrdersQueryHandler handler)
    {
        _handler = handler;
    }

    [HttpGet(ApiRoutes.Sales.GetMyOrders, Name = "get-my-orders")]
    [Tags(ApiRoutes.Sales.Tag)]
    public async Task<IActionResult> GetMyOrders([FromRoute] int tenantId, [FromRoute] int customerId, CancellationToken cancellationToken = default)
    {
        GetMyOrdersQuery query = new(customerId) { TenantId = tenantId };
        AppResponse<IEnumerable<GetOrderResponse>> response = await _handler.Handle(query, cancellationToken);
        return this.From(response);
    }
}

[ApiController]
public sealed class PlaceOrderController : ControllerBase
{
    private readonly PlaceOrderCommandHandler _handler;

    public PlaceOrderController(PlaceOrderCommandHandler handler)
    {
        _handler = handler;
    }

    [HttpPost(ApiRoutes.Sales.PlaceOrder, Name = "place-order")]
    [Tags(ApiRoutes.Sales.Tag)]
    public async Task<IActionResult> PlaceOrder([FromRoute] int tenantId, [FromBody] PlaceOrderRequest request, CancellationToken cancellationToken = default)
    {
        PlaceOrderCommand command = new(request, tenantId);
        AppResponse<bool> response = await _handler.Handle(command, cancellationToken);
        return response.IsSuccess
            ? Ok("Order placed successfully.")
            : BadRequest(response.ErrorMessages);
    }
}

// ======= Handlers =======
public sealed class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, IEnumerable<GetOrderResponse>>
{
    private readonly GetDbConnection _getDbConnection;

    public GetOrdersQueryHandler(GetDbConnection getDbConnection) => _getDbConnection = getDbConnection;

    public async ValueTask<AppResponse<IEnumerable<GetOrderResponse>>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        Tenant tenant = Tenant.FromId(request.TenantId);
        using IDbConnection db = _getDbConnection(tenant);
        string sql = @$"
                SELECT * FROM {OrdersByTenant.Get(tenant)}";
        IEnumerable<GetOrderResponse> orders = await db.QueryAsync<GetOrderResponse>(sql);
        return AppResponse<IEnumerable<GetOrderResponse>>.Succeeded(orders);
    }
}

public sealed class GetOrderQueryHandler : IRequestHandler<GetOrderQuery, GetOrderResponse>
{
    private readonly GetDbConnection _getDbConnection;

    public GetOrderQueryHandler(GetDbConnection getDbConnection) => _getDbConnection = getDbConnection;

    public async ValueTask<AppResponse<GetOrderResponse>> Handle(GetOrderQuery request, CancellationToken cancellationToken)
    {
        Tenant tenant = Tenant.FromId(request.TenantId);
        using IDbConnection db = _getDbConnection(tenant);
        string sql = @$"
                SELECT * FROM {OrdersByTenant.Get(tenant)}
                WHERE id = :id";

        DynamicParameters parameters = new();
        parameters.Add(":id", request.Id);

        GetOrderResponse? order = await db.QueryFirstOrDefaultAsync<GetOrderResponse>(sql, parameters);
        return order is null
            ? AppResponse<GetOrderResponse>.Failed($"Order with ID {request.Id} not found.")
            : AppResponse<GetOrderResponse>.Succeeded(order);
    }
}

public sealed class GetMyOrdersQueryHandler : IRequestHandler<GetMyOrdersQuery, IEnumerable<GetOrderResponse>>
{
    private readonly GetDbConnection _getDbConnection;

    public GetMyOrdersQueryHandler(GetDbConnection getDbConnection) => _getDbConnection = getDbConnection;

    public async ValueTask<AppResponse<IEnumerable<GetOrderResponse>>> Handle(GetMyOrdersQuery request, CancellationToken cancellationToken)
    {
        Tenant tenant = Tenant.FromId(request.TenantId);
        using IDbConnection db = _getDbConnection(tenant);
        string sql = @$"
                SELECT * FROM {OrdersByTenant.Get(tenant)}
                WHERE customer_id = :customerId";

        DynamicParameters parameters = new();
        parameters.Add(":customerId", request.CustomerId);

        IEnumerable<GetOrderResponse> orders = await db.QueryAsync<GetOrderResponse>(sql, parameters);
        return AppResponse<IEnumerable<GetOrderResponse>>.Succeeded(orders);
    }
}

public sealed class PlaceOrderCommandHandler : IRequestHandler<PlaceOrderCommand, bool>
{
    private readonly GetDbConnection _getDbConnection;

    public PlaceOrderCommandHandler(GetDbConnection getDbConnection)
    {
        _getDbConnection = getDbConnection;
    }

    public async ValueTask<AppResponse<bool>> Handle(PlaceOrderCommand request, CancellationToken cancellationToken)
    {
        Tenant tenant = Tenant.FromId(request.TenantId);
        using IDbConnection db = _getDbConnection(tenant);

        const string procedureName = "PLACE_ORDER";

        decimal orderId = GenerateRandomOrderId(request.TenantId);

        DynamicParameters parameters = new();
        parameters.Add("p_order_id", orderId, DbType.Int64, ParameterDirection.Input);
        parameters.Add("p_customer_id", request.Request.CustomerId, DbType.Int32, ParameterDirection.Input);
        parameters.Add("p_address", request.Request.Address, DbType.String, ParameterDirection.Input);
        parameters.Add("p_items_csv", request.Request.ItemsCsv, DbType.String, ParameterDirection.Input);
        parameters.Add("is_success", dbType: DbType.Int32, direction: ParameterDirection.Output);
        parameters.Add("p_created_on", DateTime.UtcNow, DbType.DateTime, ParameterDirection.Input);

        await db.ExecuteAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);

        int isSuccess = parameters.Get<int>("is_success");
        return isSuccess == 1
            ? AppResponse<bool>.Succeeded(true)
            : AppResponse<bool>.Failed("Failed to place the order.");
    }

    private static decimal GenerateRandomOrderId(int tenantId)
    {
        string datePart = DateTime.UtcNow.ToString("yyyyMMdd");

        int regionId = tenantId;
        string regionPart = regionId.ToString("D2"); 

        Random random = new();
        string randomPart = string.Concat(Enumerable.Range(0, 12).Select(_ => random.Next(0, 10)));

        string orderIdString = $"{datePart}{regionPart}{randomPart}";

        return decimal.Parse(orderIdString);
    }
}

public static class OrdersByTenant
{
    public static string Get(Tenant tenant)
    {
        if(tenant == Tenant.Oltp)
        {
            return "vw_sls_orders";
        }
        return "sls_orders";
    }
}
