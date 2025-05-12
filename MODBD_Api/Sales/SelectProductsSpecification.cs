using Dapper;
using MODBD_Api.Common;
using MODBD_Api.Common.Contracts;
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
        IDbConnection db,
        RowOffset rowOffset, RowCount rowCount
    ) => (await db.QueryAsync<ProductResponse>(AsQuery(rowOffset, rowCount)));

    private string AsQuery(RowOffset rowOffset, RowCount rowCount)
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
            Order By p.created_on Desc
            Offset {rowOffset.Value} Rows
            Fetch Next {rowCount.Value} Rows Only
        ";
    }
}
