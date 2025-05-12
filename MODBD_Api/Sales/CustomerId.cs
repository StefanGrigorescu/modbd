using MODBD_Common.Abstractions.Entities;

namespace MODBD_Api.Sales;

public sealed class CustomerId : EntityId<long>
{
    public static CustomerId FromValue(long value) => new() { Value = value, };
    private CustomerId() { }
}
