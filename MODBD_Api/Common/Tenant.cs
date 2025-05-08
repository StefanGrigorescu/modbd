using MODBD_Common.Abstractions.DiscriminatedUnions;

namespace MODBD_Api.Common;

public sealed class Tenant : Enumeration<Tenant>
{
    public static readonly Tenant Oltp = new() { Id = 0, Name = "Oltp", };
    public static readonly Tenant Global = new() { Id = 1, Name = "Global", };
    public static readonly Tenant Muntenia = new() { Id = 2, Name = "Muntenia", };
    public static readonly Tenant Romania = new() { Id = 3, Name = "Romania", };
    
    private Tenant() { }
}
