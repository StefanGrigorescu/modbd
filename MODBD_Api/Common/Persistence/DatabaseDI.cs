using Microsoft.Extensions.DependencyInjection;
using MODBD_Common.Collections;
using System.Text.RegularExpressions;

namespace MODBD_Api.Common.Persistence;

public static class DatabaseDI
{
    private const int _iisPort = 1515; // Default port for IIS Express

    /// <summary>
    /// Adds and configures database related services.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddDatabaseServices(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        AppReadOnlyDictionary<Tenant, string> tenantConnectionString = AppReadOnlyDictionary<Tenant, string>.New(new Dictionary<Tenant, string>()
        {
            [Tenant.Oltp] = GetConnectionString(configuration, "OltpDb"),
            [Tenant.Global] = GetConnectionString(configuration, "GlobalDb"),
            [Tenant.Muntenia] = GetConnectionString(configuration, "MunteniaDb"),
            [Tenant.Romania] = GetConnectionString(configuration, "RomaniaDb"),
        });

        services.AddScoped<GetDbConnection>(serviceProvider =>
            new GetDbConnectionImpl(tenantConnectionString).Invoke);

        //services.AddScoped<IModbdRepository, ModbdRepository>();

        return services;
    }

    private static string GetConnectionString(IConfiguration configuration, string name)
    {
        string connectionString = configuration.GetConnectionString(name) ??
            throw new Exception($"Connection string {name} not found. Check the environment variables or other configuration source where it should have been provided.");

        if(HasIisHost(configuration))
        {
            // replace host with localhost
            connectionString = ReplaceHostWithLocalhost.FromConnectionString(connectionString);
            connectionString = ReplacePortWithValue.FromConnectionString(connectionString, _iisPort);
        }

        return connectionString;
    }

    private static bool HasIisHost(IConfiguration configuration) =>
        configuration["ASPNETCORE_IIS_HTTPAUTH"] != null;
}


public static partial class ReplaceHostWithLocalhost
{
    public static string FromConnectionString(string connectionString) =>
        HostRegex()
            .Replace(
                connectionString, 
                (Match m) => $"{m.Groups[1].Value}localhost" // Use the first group (HOST=) and replace the second group (host value) with "localhost"
            );

    [GeneratedRegex("(HOST=)([^)]+)")]
    private static partial Regex HostRegex();
}


public static partial class ReplacePortWithValue
{
    public static string FromConnectionString(string connectionString, int port) =>
        PortRegex()
            .Replace(
                connectionString,
                (Match m) => $"{m.Groups[1].Value}{port}" // Use the first group (PORT=) and append the new port value
            );

    [GeneratedRegex("(PORT=)([^)]+)")]
    private static partial Regex PortRegex();
}
