using Dapper;
using Microsoft.AspNetCore.Mvc;
using MODBD_Api.Common;
using MODBD_Api.Common.Contracts;
using MODBD_Api.Common.Persistence;
using MODBD_Common.Abstractions;
using MODBD_Common.Abstractions.Responses;
using System.Data;

namespace MODBD_Api.Sales.AppQueries;

public sealed class GetOrderController : ControllerBase
{
    private readonly GetOrderQueryHandler _handler;

    public GetOrderController(GetOrderQueryHandler handler)
    {
        _handler = handler;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="tenantId">
    /// 0 - Oltp <br></br>
    /// 1 - Global <br></br>
    /// 2 - Muntenia <br></br>
    /// 3 - Romania
    /// </param>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet(ApiRoutes.Sales.GetOrder, Name = "get-order")]
    [Tags(ApiRoutes.Sales.Tag)]
    public async Task<IActionResult> GetOrder([FromRoute] int? tenantId, [FromRoute] long id, CancellationToken cancellationToken = default)
    {
        GetOrderQuery query = GetOrderQuery.From(tenantId, id);
        AppResponse<OrderResponse> response = await _handler.Handle(query, cancellationToken);
        return this.From(response);
    }
}


public sealed record GetOrderQuery : IRequest<OrderResponse>
{
    public required Tenant Tenant { get; init; }
    public required OrderId OrderId { get; init; }

    public static GetOrderQuery From(int? tenantId, long orderId) => new()
    {
        Tenant = Tenant.FromIdOrThrowIfNull(tenantId),
        OrderId = OrderId.FromValue(orderId),
    };
    private GetOrderQuery() { }
}


public sealed class GetOrderQueryHandler : IRequestHandler<GetOrderQuery, OrderResponse>
{
    private readonly GetDbConnection _getDbConnection;

    public GetOrderQueryHandler(GetDbConnection getDbConnection) => _getDbConnection = getDbConnection;

    public async ValueTask<AppResponse<OrderResponse>> Handle(GetOrderQuery request, CancellationToken cancellationToken)
    {
        using IDbConnection db = _getDbConnection(request.Tenant);

        DynamicParameters parameters = new DynamicParameters()
            .WithParameter(":id", request.OrderId);

        OrderResponse? order = (await SelectOrdersWithItemsSpecification
            .FromTenant(request.Tenant)
            .Where("id = :id")
            .QueryAsync(db, parameters))
            .FirstOrDefault();

        return order is null ? 
            AppResponse<OrderResponse>.Failed($"Order with ID {request.OrderId} not found.") : 
            AppResponse<OrderResponse>.Succeeded(order);
    }
}
