using MODBD_Analiza.Applications;

namespace MODBD_Analiza.EShop;

internal sealed record IdntUsersApplications : EntityApplications<IdntUsers>
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
            .Where(u => u.Email.Equal(new SqlQueryParameter("p_email"))),

            GetDetailsAfterLoginWithEmail = eshop
            .IdntUsers
            .Select(u => [u.Username, u.FirstName, u.LastName, u.DateOfBirth, u.PhoneNumber, u.RegionId, u.CreatedOn, u.LastUpdatedOn])
            .Where(u => u.Id.Equal(new SqlQueryParameter("p_id"))),

            LoginWithUsername = eshop
            .IdntUsers
            .Select(u => [u.Id, u.Salt, u.Password])
            .Where(u => u.Username.Equal(new SqlQueryParameter("p_username"))),

            GetDetailsAfterLoginWithUsername = eshop
            .IdntUsers
            .Select(u => [u.Email, u.FirstName, u.LastName, u.DateOfBirth, u.PhoneNumber, u.RegionId, u.CreatedOn, u.LastUpdatedOn])
            .Where(u => u.Id.Equal(new SqlQueryParameter("p_id"))),

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
