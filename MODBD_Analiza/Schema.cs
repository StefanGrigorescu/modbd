namespace MODBD_Analiza;

internal sealed class EShopSchema
{
    public required Table IdntUsers { get; init; } = new()
    {
        Name = "IDNT_USERS",
        Columns = [
            new () { Name = "id", },
            new () { Name = "username", },
            new () { Name = "first_name", },
            new () { Name = "last_name", },
            new () { Name = "date_of_birth", },
            new () { Name = "email", },
            new () { Name = "phone_number", },
            new () { Name = "password", },
            new () { Name = "salt", },
            new () { Name = "region_id", },
            new () { Name = "created_on", },
            new () { Name = "last_updated_on", },
        ],
    };

    public required Table SlsOrders { get; init; } = new()
    {
        Name = "SLS_ORDERS",
        Columns = [
            new () { Name = "id", },
            new () { Name = "customer_id", },
            new () { Name = "customer_region_id", },
            new () { Name = "address", },
            new () { Name = "status_id", },
            new () { Name = "created_on", },
            new () { Name = "last_updated_on", },
        ],
    };

    public required Table SlsOrderItems { get; init; } = new()
    {
        Name = "SLS_ORDER_ITEMS",
        Columns = [
            new () { Name = "order_id", },
            new () { Name = "product_id", },
            new () { Name = "quantity", },
            new () { Name = "created_on", },
            new () { Name = "last_updated_on", },
        ],
    };
}


internal sealed record Table
{
    public required string Name { get; init; }
    public required IReadOnlyList<Column> Columns { get; init; } = [];
}


internal sealed record Column
{
    public required string Name { get; init; }
    //public required string Type { get; init; }
    //public required bool IsPrimaryKey { get; init; }
    //public required bool IsForeignKey { get; init }
    //public required string ForeignKeyTable { get; init; }
    //public required string ForeignKeyColumn { get; init; }
}
