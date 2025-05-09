using MODBD_Api.Common;

namespace MODBD_Api.Sales;

public static class SelectProducts
{
    public static string FromTenant(Tenant tenant)
    {
        string products = tenant == Tenant.Global ?
            "vw_sls_products" :
            "sls_products";

        return $@"
            Select p.*
            From {products} p
        ";
    }
}
