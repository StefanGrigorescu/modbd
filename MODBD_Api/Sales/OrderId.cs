using MODBD_Common.Abstractions.Entities;

namespace MODBD_Api.Sales;

public sealed class OrderId : EntityId<long>
{
    public static OrderId FromValue(long value) => new() { Value = value, };
    private OrderId() { }
}
