using MODBD_Analiza.Schema;
using System.Diagnostics.CodeAnalysis;

namespace MODBD_Analiza.EShop;

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
