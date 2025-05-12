using Microsoft.AspNetCore.Mvc;
using MODBD_Api.Common;
using MODBD_Api.Common.Contracts;
using MODBD_Api.Common.Persistence;
using MODBD_Common.Abstractions;
using MODBD_Common.Abstractions.Responses;
using System.Data;

namespace MODBD_Api.Sales.AppQueries;

public sealed class GetOrdersController : ControllerBase
{
    private readonly GetOrdersQueryHandler _handler;

    public GetOrdersController(GetOrdersQueryHandler handler)
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
    /// </param>    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet(ApiRoutes.Sales.GetOrders, Name = "get-orders")]
    [Tags(ApiRoutes.Sales.Tag)]
    public async Task<IActionResult> GetOrders([FromRoute] int? tenantId, [FromQuery] GetOrdersRequest request, CancellationToken cancellationToken = default)
    {
        GetOrdersQuery query = GetOrdersQuery.From(tenantId, request);
        AppResponse<IEnumerable<OrderResponse>> response = await _handler.Handle(query, cancellationToken);
        return this.From(response);
    }
}


public sealed record GetOrdersRequest : PaginatedRequest;


public sealed record GetOrdersQuery : IRequest<IEnumerable<OrderResponse>>
{
    public required Tenant Tenant { get; init; }
    public required RowOffset RowOffset { get; init; }
    public required RowCount RowCount { get; init; }

    public static GetOrdersQuery From(int? tenantId, GetOrdersRequest request) => new()
    {
        Tenant = Tenant.FromIdOrThrowIfNull(tenantId),
        RowOffset = RowOffset.FromNullableValue(request.RowOffset),
        RowCount = RowCount.FromNullableValue(request.RowCount),
    };
    private GetOrdersQuery() { }
}


public sealed class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, IEnumerable<OrderResponse>>
{
    private readonly GetDbConnection _getDbConnection;

    public GetOrdersQueryHandler(GetDbConnection getDbConnection) => _getDbConnection = getDbConnection;

    public async ValueTask<AppResponse<IEnumerable<OrderResponse>>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        using IDbConnection db = _getDbConnection(request.Tenant);

        IEnumerable<OrderResponse> orders = await SelectOrdersWithItemsSpecification
            .FromTenant(request.Tenant)
            .QueryAsync(db, request.RowOffset, request.RowCount);
        return AppResponse<IEnumerable<OrderResponse>>.Succeeded(orders);
    }
}
