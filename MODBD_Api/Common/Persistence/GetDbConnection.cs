using MODBD_Common.Collections;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace MODBD_Api.Common.Persistence;

public class GetDbConnectionImpl
{
    private readonly AppReadOnlyDictionary<Tenant, string> _tenantConnectionString;

    public GetDbConnectionImpl(AppReadOnlyDictionary<Tenant, string> tenantConnectionString)
    {
        _tenantConnectionString = tenantConnectionString;
    }

    public IDbConnection Invoke(Tenant tenant) =>
        _tenantConnectionString.TryGetValue(tenant, out string? connectionString) ?
        new OracleConnection(connectionString) :
        throw new ArgumentException($"Tenant {tenant} not found in connection string dictionary.", nameof(tenant));
}


public delegate IDbConnection GetDbConnection(Tenant tenant);
