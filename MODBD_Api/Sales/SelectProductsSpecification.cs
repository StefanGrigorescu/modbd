using Dapper;
using MODBD_Api.Common;
using System.Data;

namespace MODBD_Api.Sales;

public sealed class SelectProductsSpecification
{
    public required Tenant Tenant { get; init; }
    private string Products => Tenant == Tenant.Global ?
        "vw_sls_products" :
        "sls_products";

    private SelectProductsSpecification() { }

    public static SelectProductsSpecification FromTenant(Tenant tenant) => new() { Tenant = tenant };

    public async Task<IEnumerable<ProductResponse>> QueryAsync(
        IDbConnection db
    ) => (await db.QueryAsync<ProductResponse>(AsQuery()));

    private string AsQuery()
    {
        return $@"
            Select 
                p.id AS Id, 
                p.name AS Name, 
                p.description AS Description, 
                p.price_in_eur AS PriceInEur, 
                p.created_on AS CreatedOn, 
                p.last_updated_on AS LastUpdatedOn
            From {Products} p
        ";
    }
}
