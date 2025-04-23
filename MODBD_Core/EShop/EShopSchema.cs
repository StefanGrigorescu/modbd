using System.Diagnostics.CodeAnalysis;

namespace MODBD_Core.EShop;

public sealed record EShopSchema
{
    public required IdntUsers IdntUsers { get; init; } = IdntUsers.New();
    public required SlsOrders SlsOrders { get; init; } = SlsOrders.New();
    public required SlsOrderItems SlsOrderItems { get; init; } = SlsOrderItems.New();

    public static EShopSchema New() => new();
    [SetsRequiredMembers] private EShopSchema() { }
}
