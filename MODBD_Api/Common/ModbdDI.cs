using MODBD_Api.Common.Persistence;
using MODBD_Api.Common.Web;

namespace MODBD_Api.Common;

public static class ModbdDI
{
    public static IServiceCollection AddModbdServices(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        services.AddApiCommonModuleServices(configuration, environment);

        return services;
    }

    private static IServiceCollection AddApiCommonModuleServices(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        services.AddDatabaseServices(configuration, environment);
        services.AddWebServices(configuration);

        return services;
    }
}
