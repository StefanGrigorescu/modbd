using MODBD_Core.Schema;
using System.Diagnostics.CodeAnalysis;

namespace MODBD_Core.EShop;

public sealed record SlsOrderItems : Table<SlsOrderItems>
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
