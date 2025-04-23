using MODBD_Api.Common.Abstractions.Responses;
using System.Reflection;

namespace MODBD_Api.Common.Abstractions;

public interface IRequestHandler<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    ValueTask<AppResponse<TResponse>> Handle(TRequest request, CancellationToken cancellationToken);
}

public interface IRequest<TResponse> { }


public interface IRequestHandler<TRequest>
    where TRequest : IRequest
{
    ValueTask<AppResponse> Handle(TRequest request, CancellationToken cancellationToken);
}

public interface IRequest { }


public static class CqrsDI
{
    public static IServiceCollection AddCQRS(this IServiceCollection services, Assembly assembly)
    {
        IEnumerable<Type> handlerTypes = assembly
            .GetTypes()
            .Where((Type type) =>
                !type.IsInterface &&
                !type.IsAbstract &&
                type
                    .GetInterfaces()
                    .Any((Type i) =>
                        i.IsGenericType &&
                        (i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>) || i.GetGenericTypeDefinition() == typeof(IRequestHandler<>))
                    )
            );

        foreach (Type handlerType in handlerTypes)
        {
            services.AddScoped(handlerType);
        }

        return services;
    }
}
