using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace SharedKernel;


public interface IRequest<TResponse> { }

public interface IQuery<TResponse> : IRequest<TResponse> { }

public interface ICommand<TResponse> : IRequest<TResponse> { }


public interface IRequestHandler<in TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
}

public interface IMediator
{
    Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken);
}


public class Mediator(IServiceProvider serviceProvider) : IMediator
{
    public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken)
    {
        var requestHandlerTypeDefinition = typeof(IRequestHandler<,>);

        var requestHandlerType = requestHandlerTypeDefinition
            .MakeGenericType(request.GetType(), typeof(TResponse));

        var requestHandler = serviceProvider.GetRequiredService(requestHandlerType);

        return requestHandler is not IRequestHandler<IRequest<TResponse>, TResponse> handler
            ? throw new InvalidOperationException($"No handler registered for request type {request.GetType().Name}")
            : handler.Handle(request, cancellationToken);
    }
}

public static class MediatorExtensions
{
    public static IServiceCollection AddMediator(
        this IServiceCollection services,
        params IEnumerable<Assembly> assemblies)
    {
        services.AddScoped<IMediator, Mediator>();

        var requestHandlerTypeDefinition = typeof(IRequestHandler<,>);

        var types = assemblies.SelectMany(assembly => assembly.GetTypes())
            .Where(c => !c.IsAbstract && !c.IsInterface)
            .SelectMany(type => type.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == requestHandlerTypeDefinition)
                .Select(i => new
                {
                    Interface = i,
                    Implementation = type,
                }));

        foreach (var item in types)
        {
            services.AddScoped(item.Interface, item.Implementation);
        }

        return services;
    }

    public static IServiceCollection AddMediator(this IServiceCollection services)
    {
        return AddMediator(services, Assembly.GetCallingAssembly());
    }
}
