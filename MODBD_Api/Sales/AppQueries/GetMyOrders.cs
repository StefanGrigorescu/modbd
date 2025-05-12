using Dapper;
using Microsoft.AspNetCore.Mvc;
using MODBD_Api.Common;
using MODBD_Api.Common.Contracts;
using MODBD_Api.Common.Persistence;
using MODBD_Common.Abstractions;
using MODBD_Common.Abstractions.Responses;
using System.Data;

namespace MODBD_Api.Sales.AppQueries;

public sealed class GetMyOrdersController : ControllerBase
{
    private readonly GetMyOrdersQueryHandler _handler;

    public GetMyOrdersController(GetMyOrdersQueryHandler handler)
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
    /// <param name="customerId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet(ApiRoutes.Sales.GetMyOrders, Name = "get-my-orders")]
    [Tags(ApiRoutes.Sales.Tag)]
    public async Task<IActionResult> GetMyOrders([FromRoute] int? tenantId, [FromRoute] long customerId, CancellationToken cancellationToken = default)
    {
        GetMyOrdersQuery query = GetMyOrdersQuery.From(tenantId: tenantId , customerId: customerId);
        AppResponse<IEnumerable<OrderResponse>> response = await _handler.Handle(query, cancellationToken);
        return this.From(response);
    }
}


public sealed record GetMyOrdersQuery : IRequest<IEnumerable<OrderResponse>>
{
    public required Tenant Tenant { get; init; }
    public required CustomerId CustomerId { get; init; }

    public static GetMyOrdersQuery From(int? tenantId, long customerId) => new()
    {
        Tenant = Tenant.FromIdOrThrowIfNull(tenantId),
        CustomerId = CustomerId.FromValue(customerId),
    };
    private GetMyOrdersQuery() { }
}


public sealed class GetMyOrdersQueryHandler : IRequestHandler<GetMyOrdersQuery, IEnumerable<OrderResponse>>
{
    private readonly GetDbConnection _getDbConnection;

    public GetMyOrdersQueryHandler(GetDbConnection getDbConnection) => _getDbConnection = getDbConnection;

    public async ValueTask<AppResponse<IEnumerable<OrderResponse>>> Handle(GetMyOrdersQuery request, CancellationToken cancellationToken)
    {
        using IDbConnection db = _getDbConnection(request.Tenant);

        DynamicParameters parameters = new DynamicParameters()
            .WithParameter(":customerId", request.CustomerId);

        IEnumerable<OrderResponse> orders = await SelectOrdersWithItemsSpecification
            .FromTenant(request.Tenant)
            .Where("customer_id = :customerId")
            .QueryAsync(db, parameters);

        return AppResponse<IEnumerable<OrderResponse>>.Succeeded(orders);
    }
}
