namespace MODBD_Analiza;

internal sealed record EShopApplications
{
    public required Application LoginWithEmail { get; init; }
    public required Application GetDetailsAfterLoginWithEmail { get; init; }
    public required Application LoginWithUsername { get; init; }
    public required Application GetDetailsAfterLoginWithUsername { get; init; }
    public required Application GetOrdersInRegion { get; init; }
    public required Application GetOrderItemsInRegion { get; init; }

    public static EShopApplications All(EShopSchema schema) => new()
    {
        LoginWithEmail = new()
        {
            DisplayedQuery = "",
            Columns = [
                schema.IdntUsers.Id,
                schema.IdntUsers.Salt,
                schema.IdntUsers.Password,
                schema.IdntUsers.Email,
            ],
        },
        GetDetailsAfterLoginWithEmail = new()
        {
            DisplayedQuery = "",
            Columns = [
                schema.IdntUsers.Id,
                schema.IdntUsers.Username,
                schema.IdntUsers.FirstName,
                schema.IdntUsers.LastName,
                schema.IdntUsers.DateOfBirth,
                schema.IdntUsers.PhoneNumber,
                schema.IdntUsers.RegionId,
                schema.IdntUsers.CreatedOn,
                schema.IdntUsers.LastUpdatedOn,
            ],
        },
        LoginWithUsername = new()
        {
            DisplayedQuery = "",
            Columns = [
                schema.IdntUsers.Id,
                schema.IdntUsers.Salt,
                schema.IdntUsers.Password,
                schema.IdntUsers.Username,
            ],
        },
        GetDetailsAfterLoginWithUsername = new()
        {
            DisplayedQuery = "",
            Columns = [
                schema.IdntUsers.Id,
                schema.IdntUsers.Email,
                schema.IdntUsers.FirstName,
                schema.IdntUsers.LastName,
                schema.IdntUsers.DateOfBirth,
                schema.IdntUsers.PhoneNumber,
                schema.IdntUsers.RegionId,
                schema.IdntUsers.CreatedOn,
                schema.IdntUsers.LastUpdatedOn,
            ],
        },
        GetOrdersInRegion = new()
        {
            DisplayedQuery = "",
            Columns = [
                schema.SlsOrders.Id,
                schema.SlsOrders.CustomerId,
                schema.SlsOrders.CustomerRegionId,
                schema.SlsOrders.Address,
                schema.SlsOrders.StatusId,
                schema.SlsOrders.CreatedOn,
                schema.SlsOrders.LastUpdatedOn,
            ],
        },
        GetOrderItemsInRegion = new()
        {
            DisplayedQuery = "",
            Columns = [
                schema.SlsOrderItems.OrderId,
                schema.SlsOrderItems.ProductId,
                schema.SlsOrderItems.Quantity,
                schema.SlsOrderItems.CreatedOn,
                schema.SlsOrderItems.LastUpdatedOn,
            ],
        },
    };
}


internal sealed record Application
{
    public required string DisplayedQuery { get; init; }
    public required IReadOnlyList<Column> Columns { get; init; }
}
