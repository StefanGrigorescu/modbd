using MODBD_Common.Abstractions;
using System.Reflection;

namespace MODBD_Api.Common.DI;

public static class CqrsDI
{
    public static IServiceCollection AddCQRS(this IServiceCollection services, Assembly assembly)
    {
        IEnumerable<Type> handlerTypes = HandlerTypes.InAssembly(assembly);

        foreach (Type handlerType in handlerTypes)
        {
            services.AddScoped(handlerType);
        }

        return services;
    }
}
