using MODBD_Core.Applications;

namespace MODBD_Core.EShop;

public sealed record IdntUsersApplications : EntityApplications<IdntUsers>
{
    public required Application<IdntUsers> LoginWithEmail { get; init; }
    public required Application<IdntUsers> GetDetailsAfterLoginWithEmail { get; init; }
    public required Application<IdntUsers> LoginWithUsername { get; init; }
    public required Application<IdntUsers> GetDetailsAfterLoginWithUsername { get; init; }

    public static IdntUsersApplications New(EShopSchema eshop)
    {
        IdntUsersApplications apps = new()
        {
            LoginWithEmail = eshop
                .IdntUsers
                .Select(u => [u.Id, u.Salt, u.Password])
                .Where(u => u.Email.Equal(new SqlQueryParameter("p_email")))
                .WithFrequencyPerMonth(60_000_000)
                .WithSelectivity(1),

            GetDetailsAfterLoginWithEmail = eshop
                .IdntUsers
                .Select(u => [u.Username, u.FirstName, u.LastName, u.DateOfBirth, u.PhoneNumber, u.RegionId, u.CreatedOn, u.LastUpdatedOn])
                .Where(u => u.Id.Equal(new SqlQueryParameter("p_id")))
                .WithFrequencyPerMonth(55_000_000)
                .WithSelectivity(1),

            LoginWithUsername = eshop
                .IdntUsers
                .Select(u => [u.Id, u.Salt, u.Password])
                .Where(u => u.Username.Equal(new SqlQueryParameter("p_username")))
                .WithFrequencyPerMonth(500_000)
                .WithSelectivity(1),

            GetDetailsAfterLoginWithUsername = eshop
                .IdntUsers
                .Select(u => [u.Email, u.FirstName, u.LastName, u.DateOfBirth, u.PhoneNumber, u.RegionId, u.CreatedOn, u.LastUpdatedOn])
                .Where(u => u.Id.Equal(new SqlQueryParameter("p_id")))
                .WithFrequencyPerMonth(483_000)
                .WithSelectivity(1),

            All = [],
            AllSimplePredicates = [],
        };

        IReadOnlyList<IApplication> all = apps.GetAllApplications();
        IReadOnlyList<ICondition> allSimplePredicates = all.GetAllSimplePredicates();

        return apps with
        {
            All = all,
            AllSimplePredicates = allSimplePredicates,
        };
    }

    private IdntUsersApplications() { }
}
