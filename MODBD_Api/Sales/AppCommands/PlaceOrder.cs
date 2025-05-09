using Dapper;
using Microsoft.AspNetCore.Mvc;
using MODBD_Api.Common;
using MODBD_Api.Common.Contracts;
using MODBD_Api.Common.Persistence;
using MODBD_Common.Abstractions;
using MODBD_Common.Abstractions.Responses;
using MODBD_Common.NumericTypes.Random;
using MODBD_Common.Time;
using System.Data;

namespace MODBD_Api.Sales.AppCommands;

public sealed class PlaceOrderController : ControllerBase
{
    private readonly PlaceOrderCommandHandler _handler;

    public PlaceOrderController(PlaceOrderCommandHandler handler)
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
    /// </param>    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost(ApiRoutes.Sales.PlaceOrder, Name = "place-order")]
    [Tags(ApiRoutes.Sales.Tag)]
    public async Task<IActionResult> PlaceOrder([FromRoute] int tenantId, [FromBody] PlaceOrderRequest request, CancellationToken cancellationToken = default)
    {
        PlaceOrderCommand command = PlaceOrderCommand.From(request, tenantId);
        AppResponse<PlaceOrderResponse> response = await _handler.Handle(command, cancellationToken);
        return this.From(response);
    }
}


public sealed record PlaceOrderRequest
{
    public required int CustomerId { get; init; }
    public required string Address { get; init; }
    /// <summary>
    /// {item_id1}x{quantity1},{item_id2xquantity2} <br></br> 
    /// Try seed item_ids are between 1-100. <br></br>
    /// Example: <br></br>
    /// 20x2,44x1,55x3
    /// </summary>
    public required string ItemsCsv { get; init; }
}


public sealed record PlaceOrderResponse
{
    public required long OrderId { get; init; }
    public required DateTime CreatedOn { get; init; }
}


public sealed record PlaceOrderCommand : IRequest<PlaceOrderResponse>
{
    public required int CustomerId { get; init; }
    public required string Address { get; init; }
    public required string ItemsCsv { get; init; }
    public required Tenant Tenant { get; init; }

    public static PlaceOrderCommand From(PlaceOrderRequest request, int tenantId) => new()
    {
        CustomerId = request.CustomerId,
        Address = request.Address,
        ItemsCsv = request.ItemsCsv,
        Tenant = Tenant.FromId(tenantId),
    };
    private PlaceOrderCommand() { }
}


public sealed class PlaceOrderCommandHandler : IRequestHandler<PlaceOrderCommand, PlaceOrderResponse>
{
    private readonly GetDbConnection _getDbConnection;
    private readonly IRandom _random;
    private readonly Utc.Snapshot _utcSnapshot;

    public PlaceOrderCommandHandler(
        GetDbConnection getDbConnection,
        IRandom random,
        Utc.Snapshot utcSnapshot
    ) {
        _getDbConnection = getDbConnection;
        _random = random;
        _utcSnapshot = utcSnapshot;
    }

    public async ValueTask<AppResponse<PlaceOrderResponse>> Handle(PlaceOrderCommand request, CancellationToken cancellationToken)
    {
        using IDbConnection db = _getDbConnection(request.Tenant);

        const string procedureName = "PLACE_ORDER";

        long orderId = SnowflakeId.New(request.Tenant, _random, _utcSnapshot);

        DynamicParameters parameters = new();
        parameters.Add("p_order_id", orderId, DbType.Int64, ParameterDirection.Input);
        parameters.Add("p_customer_id", request.CustomerId, DbType.Int32, ParameterDirection.Input);
        parameters.Add("p_address", request.Address, DbType.String, ParameterDirection.Input);
        parameters.Add("p_items_csv", request.ItemsCsv, DbType.String, ParameterDirection.Input);
        parameters.Add("is_success", dbType: DbType.Int32, direction: ParameterDirection.Output);
        parameters.Add("p_created_on", DateTime.UtcNow, DbType.DateTime, ParameterDirection.Input);

        await db.ExecuteAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);

        int isSuccess = parameters.Get<int>("is_success");
        return isSuccess == 1 ?
            AppResponse<PlaceOrderResponse>.Succeeded(new()
            {
                OrderId = orderId,
                CreatedOn = _utcSnapshot.AsDateTime(),
            }) :
            AppResponse<PlaceOrderResponse>.Failed("Failed to place the order.");
    }
}
