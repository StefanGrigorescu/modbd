using MODBD_Api.Common.Persistence;
using MODBD_Api.Common.Web;

namespace MODBD_Api.Common.DI;

public static class ModbdDI
{
    public static IServiceCollection AddModbdServices(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        services.AddCommonLibraryServices();
        services.AddApiCommonModuleServices(configuration, environment); 
        
        services.AddCQRS(typeof(Program).Assembly);

        return services;
    }

    private static IServiceCollection AddApiCommonModuleServices(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        services.AddDatabaseServices(configuration, environment);
        services.AddWebServices(configuration);

        return services;
    }

    /// <summary>
    /// Services from Omega.Common library. They have no dependencies other than Microsoft.Extensions.DependencyInjection.Abstractions.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    private static IServiceCollection AddCommonLibraryServices(this IServiceCollection services)
    {
        services.AddTimeServices();
        services.AddGeneratorServices();

        return services;
    }
}
