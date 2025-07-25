using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ToDoList.Application.Common.Interfaces;
using ToDoList.Infrastructure.Persistence;
using ToDoList.Infrastructure.Persistence.Repositories;

namespace ToDoList.Infrastructure;

/// <summary>
/// Provides extension methods for service registration in the Infrastructure layer.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds infrastructure layer services to the specified IServiceCollection.
    /// </summary>
    /// <param name="services">The IServiceCollection to add the services to.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection"),
                builder => builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        services.AddScoped<IToDoItemRepository, ToDoItemRepository>();

        return services;
    }
}