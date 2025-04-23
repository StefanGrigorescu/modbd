using MODBD_Common.Abstractions.Responses;
using System.Reflection;

namespace MODBD_Common.Abstractions;

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


public static class HandlerTypes
{
    public static IEnumerable<Type> InAssembly(Assembly assembly) => assembly
        .GetTypes()
        .Where((type) =>
            !type.IsInterface &&
            !type.IsAbstract &&
            type
                .GetInterfaces()
                .Any((i) =>
                    i.IsGenericType &&
                    (i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>) || i.GetGenericTypeDefinition() == typeof(IRequestHandler<>))
                )
        );
}
