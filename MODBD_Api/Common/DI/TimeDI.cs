using MODBD_Common.Time;

namespace MODBD_Api.Common.DI;

public static class TimeDI
{
    /// <summary>
    /// Adds and configures time services.
    /// </summary>
    /// <param name="services"></param>
    /// 
    /// <returns></returns>
    public static IServiceCollection AddTimeServices(this IServiceCollection services)
    {
        // Utc
        services.AddSingleton<UtcNow>(_ => () => Utc.Now);
        services.AddScoped<Utc.Snapshot>();

        // LocalDateTime
        services.AddSingleton<LocalDateTimeNow>(_ => () => LocalDateTime.Now);
        services.AddScoped<LocalDateTime.Snapshot>();

        return services;
    }
}
