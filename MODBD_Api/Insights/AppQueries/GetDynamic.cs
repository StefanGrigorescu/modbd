using Dapper;
using Microsoft.AspNetCore.Mvc;
using MODBD_Api.Common;
using MODBD_Api.Common.Contracts;
using MODBD_Api.Common.Persistence;
using MODBD_Common.Abstractions;
using MODBD_Common.Abstractions.Responses;
using MODBD_Common.Text;
using System.Data;

namespace MODBD_Api.Insights.AppQueries;

public sealed class GetDynamicController : ControllerBase
{
    private readonly GetDynamicQueryHandler _handler;

    public GetDynamicController(GetDynamicQueryHandler handler)
    {
        _handler = handler;
    }

    /// <summary>
    /// Convenient method to dynamically read data from database with a custom query.
    /// </summary>
    /// <param name="tenantId">
    /// 0 - Oltp <br></br>
    /// 1 - Global <br></br>
    /// 2 - Muntenia <br></br>
    /// 3 - Romania
    /// </param>    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet(ApiRoutes.Insights.GetDynamic, Name = "get-dynamic")]
    [Tags(ApiRoutes.Insights.Tag)]
    public async Task<IActionResult> GetDynamic([FromRoute] int? tenantId, [FromQuery] string sql, CancellationToken cancellationToken = default)
    {
        GetDynamicQuery query = GetDynamicQuery.From(tenantId, sql);
        AppResponse<IEnumerable<object>> response = await _handler.Handle(query, cancellationToken);
        return this.From(response);
    }
}


public sealed record GetDynamicQuery : IRequest<IEnumerable<object>>
{
    public required Tenant Tenant { get; init; }
    public required TextBoxLg Sql { get; init; }

    public static GetDynamicQuery From(int? tenantId, string sql) => new() 
    { 
        Tenant = Tenant.FromIdOrThrowIfNull(tenantId), 
        Sql = TextBoxLg.From(sql), 
    };
    private GetDynamicQuery() { }
}


public sealed class GetDynamicQueryHandler : IRequestHandler<GetDynamicQuery, IEnumerable<object>>
{
    private readonly GetDbConnection _getDbConnection;

    public GetDynamicQueryHandler(GetDbConnection getDbConnection) => _getDbConnection = getDbConnection;

    public async ValueTask<AppResponse<IEnumerable<object>>> Handle(GetDynamicQuery request, CancellationToken cancellationToken)
    {
        using IDbConnection db = _getDbConnection(request.Tenant);

        IEnumerable<object> response = await db.QueryAsync<object>(request.Sql);

        return AppResponse<IEnumerable<object>>.Succeeded(response);
    }
}
