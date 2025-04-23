using MODBD_Core.Schema;
using System.Diagnostics.CodeAnalysis;

namespace MODBD_Core.EShop;

public sealed record IdntUsers : Table<IdntUsers>
{
    public required override string Name { get; init; } = "IDNT_USERS";
    public required override IReadOnlyList<Column> AllColumns { get; init; }

    public required Column Id { get; init; } = new() { Name = "id" };
    public required Column Username { get; init; } = new() { Name = "username" };
    public required Column FirstName { get; init; } = new() { Name = "first_name" };
    public required Column LastName { get; init; } = new() { Name = "last_name" };
    public required Column DateOfBirth { get; init; } = new() { Name = "date_of_birth" };
    public required Column Email { get; init; } = new() { Name = "email" };
    public required Column PhoneNumber { get; init; } = new() { Name = "phone_number" };
    public required Column Password { get; init; } = new() { Name = "password" };
    public required Column Salt { get; init; } = new() { Name = "salt" };
    public required Column RegionId { get; init; } = new() { Name = "region_id", };
    public required Column CreatedOn { get; init; } = new() { Name = "created_on", };
    public required Column LastUpdatedOn { get; init; } = new() { Name = "last_updated_on", };

    public override AliasedTable<IdntUsers> As(string alias) =>
        AliasedTable<IdntUsers>.New(this, alias);

    public static IdntUsers New() => new();
    public override IdntUsers Copy() => New();
    [SetsRequiredMembers]
    private IdntUsers()
    {
        AllColumns = GetAllColumns();
    }
}
