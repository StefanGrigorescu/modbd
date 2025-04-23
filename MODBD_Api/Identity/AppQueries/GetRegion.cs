using Dapper;
using Microsoft.AspNetCore.Mvc;
using MODBD_Api.Common.Abstractions;
using MODBD_Api.Common.Abstractions.Responses;
using MODBD_Api.Common.Contracts;
using MODBD_Api.Common.Persistence;
using System.Data;

namespace MODBD_Api.Identity.AppQueries;

public sealed class GetRegionController : ControllerBase
{
    private readonly GetRegionQueryHandler _handler;

    public GetRegionController(GetRegionQueryHandler handler)
    {
        _handler = handler;
    }

    //[Authorize(AuthorizationPolicies.JwtSchemes.AuthenticatedUser)]
    [HttpGet(ApiRoutes.Identity.GetRegion, Name = "get-region")]
    [Tags(ApiRoutes.Identity.Tag)]
    public async Task<IActionResult> GetRegion([FromRoute] int id, CancellationToken cancellationToken = default)
    {
        GetRegionQuery query = GetRegionQuery.From(id);
        AppResponse<GetRegionResponse> response = await _handler.Handle(query, cancellationToken);
        return this.From(response);
    }
}


public sealed record GetRegionResponse
{
    public required string Name { get; init; }
    public required DateTime CreatedOn { get; init; }
    public required DateTime LastUpdatedOn { get; init; }
}


public sealed record GetRegionQuery : IRequest<GetRegionResponse>
{
    public required int Id { get; init; }

    public static GetRegionQuery From(int id) => new()
    {
        Id = id,
    };
    private GetRegionQuery() { }
}


public sealed class GetRegionQueryHandler : IRequestHandler<GetRegionQuery, GetRegionResponse>
{
    private readonly GetDbConnection _getDbConnection;

    public GetRegionQueryHandler(GetDbConnection getDbConnection)
    {
        _getDbConnection = getDbConnection;
    }

    public async ValueTask<AppResponse<GetRegionResponse>> Handle(GetRegionQuery request, CancellationToken cancellationToken)
    {
        using (IDbConnection dbConnection = _getDbConnection())
        {
            string sql = @"
                    SELECT name, created_on, last_updated_on 
                    FROM IDNT_REGIONS 
                    WHERE id = :id";

            DynamicParameters parameters = new ();
            parameters.Add(":id", request.Id); 
            
            GetRegionResponse? region = await dbConnection.QueryFirstOrDefaultAsync<GetRegionResponse>(sql, parameters);

            return region is null ?
                AppResponse<GetRegionResponse>.Failed($"Region with ID {request.Id} not found.") :
                AppResponse<GetRegionResponse>.Succeeded(region);
        }
    }
}
