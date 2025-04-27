using MODBD_Core.Applications;

namespace MODBD_Core.EShop;

public sealed record IdntUsersApplications : EntityApplications<IdntUsers>
{
    //public required Application<IdntUsers> LoginWithEmail { get; init; }
    //public required Application<IdntUsers> GetDetailsAfterLoginWithEmail { get; init; }
    //public required Application<IdntUsers> LoginWithUsername { get; init; }
    //public required Application<IdntUsers> GetDetailsAfterLoginWithUsername { get; init; }

    public static IdntUsersApplications New(EShopSchema eshop)
    {
        IdntUsersApplications apps = new()
        {
            //LoginWithEmail = eshop
            //    .IdntUsers
            //    .Select(u => [u.Id, u.Salt, u.Password])
            //    .Where(u => u.Email.Equal(new SqlQueryParameter("p_email")))
            //    .WithFrequencyPerMonth(60_000_000)
            //    .WithSelectivity(1)
            //    .WithName(nameof(LoginWithEmail))
            //    .WithIsMain(),

            //GetDetailsAfterLoginWithEmail = eshop
            //    .IdntUsers
            //    .Select(u => [u.Username, u.FirstName, u.LastName, u.DateOfBirth, u.PhoneNumber, u.RegionId, u.CreatedOn, u.LastUpdatedOn])
            //    .Where(u => u.Id.Equal(new SqlQueryParameter("p_id")))
            //    .WithFrequencyPerMonth(55_000_000)
            //    .WithSelectivity(1)
            //    .WithName(nameof(GetDetailsAfterLoginWithEmail))
            //    .WithIsMain(),

            //LoginWithUsername = eshop
            //    .IdntUsers
            //    .Select(u => [u.Id, u.Salt, u.Password])
            //    .Where(u => u.Username.Equal(new SqlQueryParameter("p_username")))
            //    .WithFrequencyPerMonth(500_000)
            //    .WithSelectivity(1)
            //    .WithName(nameof(LoginWithUsername))
            //    .WithIsMain(),

            //GetDetailsAfterLoginWithUsername = eshop
            //    .IdntUsers
            //    .Select(u => [u.Email, u.FirstName, u.LastName, u.DateOfBirth, u.PhoneNumber, u.RegionId, u.CreatedOn, u.LastUpdatedOn])
            //    .Where(u => u.Id.Equal(new SqlQueryParameter("p_id")))
            //    .WithFrequencyPerMonth(483_000)
            //    .WithSelectivity(1)
            //    .WithName(nameof(GetDetailsAfterLoginWithUsername))
            //    .WithIsMain(),

            All = [],
            AllSimplePredicates = [],
            AllMain = [],
        };

        IReadOnlyList<IApplication> all = apps.GetAllApplications();
        IReadOnlyList<ICondition> allSimplePredicates = all.GetAllSimplePredicates();
        IReadOnlyList<IApplication> allMainApplications = all.GetAllMainApplications();

        return apps with
        {
            All = all,
            AllSimplePredicates = allSimplePredicates,
            AllMain = allMainApplications,
        };
    }

    private IdntUsersApplications() { }
}
