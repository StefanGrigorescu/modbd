using System.Diagnostics.CodeAnalysis;

namespace MODBD_Analiza;

internal sealed record EShopSchema
{
    public required IdntUsers IdntUsers { get; init; } = IdntUsers.New();
    public required SlsOrders SlsOrders { get; init; } = SlsOrders.New();
    public required SlsOrderItems SlsOrderItems { get; init; } = SlsOrderItems.New();

    public static EShopSchema New() => new();
    [SetsRequiredMembers] private EShopSchema() { }
}


internal sealed record IdntUsers : Table
{
    public required override string Name { get; init; } = "IDNT_USERS";
    public required override IReadOnlyList<Column> AllColumns { get; init; }

    public required Column Id = new() { Name = "id", };
    public required Column Username = new() { Name = "username", };
    public required Column FirstName = new() { Name = "first_name", };
    public required Column LastName = new() { Name = "last_name", };
    public required Column DateOfBirth = new() { Name = "date_of_birth", };
    public required Column Email = new() { Name = "email", };
    public required Column PhoneNumber = new() { Name = "phone_number", };
    public required Column Password = new() { Name = "password", };
    public required Column Salt = new() { Name = "salt", };
    public required Column RegionId = new() { Name = "region_id", };
    public required Column CreatedOn = new() { Name = "created_on", };
    public required Column LastUpdatedOn = new() { Name = "last_updated_on", };

    public static IdntUsers New() => new();
    [SetsRequiredMembers] private IdntUsers()
    {
        AllColumns = GetAllColumns();
    }
}


internal sealed record SlsOrders : Table
{
    public required override string Name { get; init; } = "SLS_ORDERS";
    public required override IReadOnlyList<Column> AllColumns { get; init; }

    public required Column Id = new() { Name = "id", };
    public required Column CustomerId { get; init; } = new() { Name = "customer_id", };
    public required Column CustomerRegionId { get; init; } = new() { Name = "customer_region_id", };
    public required Column Address = new() { Name = "address", };
    public required Column StatusId { get; init; } = new() { Name = "status_id", };
    public required Column CreatedOn = new() { Name = "created_on", };
    public required Column LastUpdatedOn = new() { Name = "last_updated_on", };

    public static SlsOrders New() => new();
    [SetsRequiredMembers] private SlsOrders()
    {
        AllColumns = GetAllColumns();
    }
}


internal sealed record SlsOrderItems : Table
{
    public required override string Name { get; init; } = "SLS_ORDER_ITEMS";
    public required override IReadOnlyList<Column> AllColumns { get; init; }

    public required Column OrderId = new() { Name = "order_id", };
    public required Column ProductId { get; init; } = new() { Name = "product_id", };
    public required Column Quantity = new() { Name = "quantity", };
    public required Column CreatedOn = new() { Name = "created_on", };
    public required Column LastUpdatedOn = new() { Name = "last_updated_on", };

    public static SlsOrderItems New() => new();
    [SetsRequiredMembers] private SlsOrderItems() 
    {
        AllColumns = GetAllColumns();
    }
}


internal abstract record Table
{
    public abstract string Name { get; init; }
    public abstract IReadOnlyList<Column> AllColumns { get; init; }

    public IReadOnlyList<Column> GetAllColumns() =>
        GetType()
            .GetProperties()
            .Where(p => p.PropertyType == typeof(Column))
            .Select(p => (p.GetValue(this) as Column)!)
            .ToArray()
            .AsReadOnly();
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
