using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace MODBD_Api.Common.Persistence;

public class GetDbConnectionImpl
{
    private readonly string _connectionString;

    public GetDbConnectionImpl(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IDbConnection Invoke() =>
        new OracleConnection(_connectionString);
}


public delegate IDbConnection GetDbConnection();
