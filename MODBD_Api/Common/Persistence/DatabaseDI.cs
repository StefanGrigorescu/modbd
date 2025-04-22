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
        string connectionString = configuration.GetConnectionString("OracleDb") ??
            throw new Exception($"Connection string not found. Check the environment variables or other configuration source where it should have been provided.");

        services.AddScoped<GetDbConnection>(serviceProvider =>
            new GetDbConnectionImpl(connectionString).Invoke);

        //services.AddScoped<IModbdRepository, ModbdRepository>();

        return services;
    }
}
