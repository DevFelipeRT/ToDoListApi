using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Application.Common.Interfaces;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Domain.Lists.Repositories;
using Domain.Accounts.Repositories;

namespace Infrastructure;

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
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                builder => builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        services.AddScoped<IDatabaseSeeder, ApplicationDbContextSeed>();

        services.AddScoped<IToDoListRepository, ToDoListRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }
}