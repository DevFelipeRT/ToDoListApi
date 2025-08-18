using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Domain.Lists.Repositories;
using Domain.Accounts.Repositories;
using Application.Common.Interfaces;
using Application.Notifications.Email;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Email;
using Infrastructure.Links;
using Application.Abstractions.Persistence;
using Application.Abstractions.Messaging;
using Infrastructure.Messaging;

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
        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<IUnitOfWork, EfUnitOfWork>();
        services.AddScoped<IDomainEventDispatcher, MediatRDomainEventDispatcher>();
        // services.AddTransient<IEmailSender, NoOpEmailSender>();
        services.AddSingleton<IEmailSender>(_ =>
            new DiskEmailSender(
                outputFolder: "var/outbox/emails",
                defaultFrom:  "no-reply@example.local"));

        services.Configure<ActivationLinkOptions>(configuration.GetSection("ActivationLink"));
        services.AddSingleton<Application.Accounts.Abstractions.IActivationLinkBuilder, ActivationLinkBuilder>();

        return services;
    }
}