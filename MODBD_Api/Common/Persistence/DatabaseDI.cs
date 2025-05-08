using Microsoft.Extensions.DependencyInjection;
using MODBD_Common.Collections;

namespace MODBD_Api.Common.Persistence;

public static class DatabaseDI
{
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

    private static string GetConnectionString(IConfiguration configuration, string name) =>
        configuration.GetConnectionString(name) ??
            throw new Exception($"Connection string {name} not found. Check the environment variables or other configuration source where it should have been provided.");
}
