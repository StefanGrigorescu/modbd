namespace MODBD_Api.Sales;

public sealed record OrderResponse
{
    public required long Id { get; init; }
    public required long CustomerId { get; init; }
    public required int CustomerRegionId { get; init; }
    public required string Address { get; init; }
    public required string Status { get; init; }
    public required DateTime CreatedOn { get; init; }
    public required DateTime LastUpdatedOn { get; init; }
    public required IReadOnlyList<ProductResponse> Items { get; init; }
    public required decimal TotalPriceInEur { get; init; }
}
