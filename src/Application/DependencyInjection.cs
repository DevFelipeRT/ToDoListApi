using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace ToDoList.Application;

/// <summary>
/// Provides extension methods for service registration in the Application layer.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds application layer services to the specified IServiceCollection.
    /// </summary>
    /// <param name="services">The IServiceCollection to add the services to.</param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg => 
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        
        return services;
    }
}