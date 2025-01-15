using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AuthHub.Runtime.Host.Configurations;

public static class EndpointConfigurator
{
    public static IServiceCollection AddEndpoints(this IServiceCollection services)
    {
        var serviceDescriptors = typeof(EndpointConfigurator).Assembly.DefinedTypes
            .Where(type => type is { IsAbstract: false, IsInterface: false } && type.IsAssignableTo(typeof(IEndpoint)))
            .Select(type => ServiceDescriptor.Transient(typeof(IEndpoint), type))
            .ToArray();

        services.TryAddEnumerable(serviceDescriptors);

        return services;
    }

    public static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder builder)
    {
        var endpoints = builder.ServiceProvider.GetRequiredService<IEnumerable<IEndpoint>>();

        foreach (var endpoint in endpoints)
        {
            endpoint.MapEndpoint(builder);
        }

        return builder;
    }
}