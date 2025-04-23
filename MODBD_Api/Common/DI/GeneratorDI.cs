using MODBD_Common.Abstractions;
using MODBD_Common.NumericTypes.Random;

namespace MODBD_Api.Common.DI;

public static class GeneratorDI
{
    /// <summary>
    /// Adds and configures number generator.
    /// </summary>
    /// <param name="services"></param>
    /// 
    /// <returns></returns>
    public static IServiceCollection AddGeneratorServices(this IServiceCollection services)
    {
        services.AddSingleton<IRandom, AppRandom>();
        services.AddSingleton<ICryptographicRandom, AppCryptographicRandom>();
        services.AddSingleton<IChance, Chance>();
        services.AddScoped<UidFactory>();   // The service is scoped, since it captures a snapshot of IDateTimeFactory.UtcNow() and uses it to generate all uids in a scope.

        return services;
    }
}
