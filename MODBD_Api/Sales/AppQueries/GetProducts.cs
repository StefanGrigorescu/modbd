using Dapper;
using Microsoft.AspNetCore.Mvc;
using MODBD_Api.Common;
using MODBD_Api.Common.Contracts;
using MODBD_Api.Common.Persistence;
using MODBD_Common.Abstractions;
using MODBD_Common.Abstractions.Responses;
using System.Data;

namespace MODBD_Api.Sales.AppQueries;

public sealed class GetProductsController : ControllerBase
{
    private readonly GetProductsQueryHandler _handler;

    public GetProductsController(GetProductsQueryHandler handler)
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
    [HttpGet(ApiRoutes.Sales.GetProducts, Name = "get-Products")]
    [Tags(ApiRoutes.Sales.Tag)]
    public async Task<IActionResult> GetProducts([FromRoute] int? tenantId, GetProductsRequest request, CancellationToken cancellationToken = default)
    {
        GetProductsQuery query = GetProductsQuery.From(tenantId, request);
        AppResponse<IEnumerable<ProductResponse>> response = await _handler.Handle(query, cancellationToken);
        return this.From(response);
    }
}


public sealed record GetProductsRequest : PaginatedRequest;


public sealed record GetProductsQuery : IRequest<IEnumerable<ProductResponse>>
{
    public required Tenant Tenant { get; init; }
    public required RowOffset RowOffset { get; init; }
    public required RowCount RowCount { get; init; }

    public static GetProductsQuery From(int? tenantId, GetProductsRequest request) => new()
    {
        Tenant = Tenant.FromIdOrThrowIfNull(tenantId),
        RowOffset = RowOffset.FromNullableValue(request.RowOffset),
        RowCount = RowCount.FromNullableValue(request.RowCount),
    };
    private GetProductsQuery() { }
}


public sealed class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, IEnumerable<ProductResponse>>
{
    private readonly GetDbConnection _getDbConnection;

    public GetProductsQueryHandler(GetDbConnection getDbConnection) => _getDbConnection = getDbConnection;

    public async ValueTask<AppResponse<IEnumerable<ProductResponse>>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        using IDbConnection db = _getDbConnection(request.Tenant);

        IEnumerable<ProductResponse> products = await SelectProductsSpecification
            .FromTenant(request.Tenant)
            .QueryAsync(db, request.RowOffset, request.RowCount);
        return AppResponse<IEnumerable<ProductResponse>>.Succeeded(products);
    }
}
