using System.Diagnostics.CodeAnalysis;
using MODBD_Analiza.Schema;

namespace MODBD_Analiza.EShop;

internal sealed record EShopSchema
{
    public required IdntUsers IdntUsers { get; init; } = IdntUsers.New();
    public required SlsOrders SlsOrders { get; init; } = SlsOrders.New();
    public required SlsOrderItems SlsOrderItems { get; init; } = SlsOrderItems.New();

    public static EShopSchema New() => new();
    [SetsRequiredMembers] private EShopSchema() { }
}


internal sealed record IdntUsers : Table<IdntUsers>
{
    public required override string Name { get; init; } = "IDNT_USERS";
    public required override IReadOnlyList<Column> AllColumns { get; init; }

    public required Column Id { get; init; } = new() { Name = "id" };
    public required Column Username { get; init; } = new() { Name = "username" };
    public required Column FirstName { get; init; } = new() { Name = "first_name" };
    public required Column LastName { get; init; } = new() { Name = "last_name" };
    public required Column DateOfBirth { get; init; } = new() { Name = "date_of_birth"   };
    public required Column Email { get; init; } = new() { Name = "email" };
    public required Column PhoneNumber { get; init; } = new() { Name = "phone_number" };
    public required Column Password { get; init; } = new() { Name = "password" };
    public required Column Salt { get; init; } = new() { Name = "salt"   };
    public required Column RegionId { get; init; } = new() { Name = "region_id", };
    public required Column CreatedOn { get; init; } = new() { Name = "created_on", };
    public required Column LastUpdatedOn { get; init; } = new() { Name = "last_updated_on", };

    public override AliasedTable<IdntUsers> As(string alias) =>
        AliasedTable<IdntUsers>.New(this, alias);

    public static IdntUsers New() => new();
    public override IdntUsers Copy() => New();
    [SetsRequiredMembers]
    private IdntUsers()
    {
        AllColumns = GetAllColumns();
    }
}


internal sealed record SlsOrders : Table<SlsOrders>
{
    public required override string Name { get; init; } = "SLS_ORDERS";
    public required override IReadOnlyList<Column> AllColumns { get; init; }

    public required Column Id { get; init; } = new() { Name = "id", };
    public required Column CustomerId { get; init; } = new() { Name = "customer_id", };
    public required Column CustomerRegionId { get; init; } = new() { Name = "customer_region_id", };
    public required Column Address { get; init; } = new() { Name = "address", };
    public required Column StatusId { get; init; } = new() { Name = "status_id", };
    public required Column CreatedOn { get; init; } = new() { Name = "created_on", };
    public required Column LastUpdatedOn { get; init; } = new() { Name = "last_updated_on", };

    public override AliasedTable<SlsOrders> As(string alias) =>
        AliasedTable<SlsOrders>.New(this, alias);

    public static SlsOrders New() => new();
    public override SlsOrders Copy() => New();
    [SetsRequiredMembers]
    private SlsOrders()
    {
        AllColumns = GetAllColumns();
    }
}


internal sealed record SlsOrderItems : Table<SlsOrderItems>
{
    public required override string Name { get; init; } = "SLS_ORDER_ITEMS";
    public required override IReadOnlyList<Column> AllColumns { get; init; }

    public required Column OrderId { get; init; } = new() { Name = "order_id", };
    public required Column ProductId { get; init; } = new() { Name = "product_id", };
    public required Column Quantity { get; init; } = new() { Name = "quantity", };
    public required Column CreatedOn { get; init; } = new() { Name = "created_on", };
    public required Column LastUpdatedOn { get; init; } = new() { Name = "last_updated_on", };

    public override AliasedTable<SlsOrderItems> As(string alias) =>
        AliasedTable<SlsOrderItems>.New(this, alias);

    public static SlsOrderItems New() => new();
    public override SlsOrderItems Copy() => New();
    [SetsRequiredMembers]
    private SlsOrderItems()
    {
        AllColumns = GetAllColumns();
    }
}
