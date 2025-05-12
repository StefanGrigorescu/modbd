using MODBD_Common.Abstractions.DiscriminatedUnions;

namespace MODBD_Api.Common;

public sealed class Tenant : Enumeration<Tenant>
{
    public static readonly Tenant Oltp = new() { Id = 0, Name = "Oltp", };
    public static readonly Tenant Global = new() { Id = 1, Name = "Global", };
    public static readonly Tenant Muntenia = new() { Id = 2, Name = "Muntenia", };
    public static readonly Tenant Romania = new() { Id = 3, Name = "Romania", };

    public static Tenant FromRegionId(int regionId) => regionId switch
    {
        0 => Muntenia,
        _ => Romania,
    };

    public static Tenant FromIdOrThrowIfNull(int? id) =>
        id is null ?
            throw new ArgumentNullException(nameof(id), "Tenant id cannot be null.") :
            FromId(id.Value);

    private Tenant() { }
}
