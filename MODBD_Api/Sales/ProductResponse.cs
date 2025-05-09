namespace MODBD_Api.Sales;

public sealed record ProductResponse
{
    public required long Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required decimal PriceInEur { get; init; }
    public required DateTime CreatedOn { get; init; }
    public required DateTime? LastUpdatedOn { get; init; }
}
