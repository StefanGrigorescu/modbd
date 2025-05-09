using Dapper;
using Microsoft.AspNetCore.Mvc;
using MODBD_Common.Abstractions;
using MODBD_Common.Abstractions.Responses;
using MODBD_Api.Common.Contracts;
using MODBD_Api.Common.Persistence;
using System.Data;
using MODBD_Api.Common;

namespace MODBD_Api.Sales.AppQueries
{
    // ======= Response DTO =======
    public sealed record GetOrderResponse
    {
        public required int      Id            { get; init; }
        public required int      CustomerId    { get; init; }
        public required DateTime OrderDate     { get; init; }
        public required decimal  TotalAmount   { get; init; }
        public required string   Status        { get; init; }
        public required DateTime CreatedOn     { get; init; }
        public required DateTime LastUpdatedOn { get; init; }
    }

    // ======= Queries =======
    public sealed record GetOrdersQuery : IRequest<IEnumerable<GetOrderResponse>>;
    public sealed record GetOrderQuery(int Id) : IRequest<GetOrderResponse>;
    public sealed record GetMyOrdersQuery(int CustomerId) : IRequest<IEnumerable<GetOrderResponse>>;

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
        public async Task<IActionResult> GetOrders(CancellationToken cancellationToken = default)
        {
            var query = new GetOrdersQuery();
            var response = await _handler.Handle(query, cancellationToken);
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
        public async Task<IActionResult> GetOrder([FromRoute] int id, CancellationToken cancellationToken = default)
        {
            var query = new GetOrderQuery(id);
            var response = await _handler.Handle(query, cancellationToken);
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
        public async Task<IActionResult> GetMyOrders([FromRoute] int customerId, CancellationToken cancellationToken = default)
        {
            var query = new GetMyOrdersQuery(customerId);
            var response = await _handler.Handle(query, cancellationToken);
            return this.From(response);
        }
    }

    // ======= Handlers =======
    public sealed class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, IEnumerable<GetOrderResponse>>
    {
        private readonly GetDbConnection _getDbConnection;

        public GetOrdersQueryHandler(GetDbConnection getDbConnection) => _getDbConnection = getDbConnection;

        public async ValueTask<AppResponse<IEnumerable<GetOrderResponse>>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
        {
            using IDbConnection db = _getDbConnection(Tenant.Global);
            const string sql = @"
                SELECT * FROM VW_SLS_ORDERS";
            var orders = await db.QueryAsync<GetOrderResponse>(sql);
            return AppResponse<IEnumerable<GetOrderResponse>>.Succeeded(orders);
        }
    }

    public sealed class GetOrderQueryHandler : IRequestHandler<GetOrderQuery, GetOrderResponse>
    {
        private readonly GetDbConnection _getDbConnection;

        public GetOrderQueryHandler(GetDbConnection getDbConnection) => _getDbConnection = getDbConnection;

        public async ValueTask<AppResponse<GetOrderResponse>> Handle(GetOrderQuery request, CancellationToken cancellationToken)
        {
            using IDbConnection db = _getDbConnection(Tenant.Global);
            const string sql = @"
                SELECT * FROM VW_SLS_ORDERS
                WHERE id = :id";

            var parameters = new DynamicParameters();
            parameters.Add(":id", request.Id);

            var order = await db.QueryFirstOrDefaultAsync<GetOrderResponse>(sql, parameters);
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
            using IDbConnection db = _getDbConnection(Tenant.Global);
            const string sql = @"
                SELECT * FROM VW_SLS_ORDERS
                WHERE customer_id = :customerId";

            var parameters = new DynamicParameters();
            parameters.Add(":customerId", request.CustomerId);

            var orders = await db.QueryAsync<GetOrderResponse>(sql, parameters);
            return AppResponse<IEnumerable<GetOrderResponse>>.Succeeded(orders);
        }
    }
}
