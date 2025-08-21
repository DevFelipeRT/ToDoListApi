using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Domain.Accounts.Policies.Interfaces;
using Domain.Accounts.Services.Interfaces;
using Domain.Lists.Policies;
using Domain.Lists.Services.Interfaces;
using Domain.Lists.Services;
using Application.Accounts.Services.Interfaces;
using Application.Accounts.Services;
using Application.Lists.Services;
using Application.Accounts.DomainEventHandlers;

namespace Application;

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
            cfg.RegisterServicesFromAssemblies(
                typeof(CreateActivationTokenOnAccountRegisteredHandler).Assembly, 
                Assembly.GetExecutingAssembly()
            ));

        // Dependency injection for domain services
        services.AddScoped<IAccountLockoutPolicy, AccountLockoutPolicy>();
        services.AddScoped<IAccountUniquenessPolicy, AccountUniquenessChecker>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IPasswordPolicy, PasswordPolicy>();
        services.AddScoped<IReminderSchedulerService, ReminderSchedulerService>();
        services.AddScoped<IToDoListItemTransferService, ToDoListItemTransferService>();
        services.AddScoped<IToDoListUniquenessPolicy, ToDoListUniquenessChecker>();
        services.AddScoped<IAuthorizationService, AuthorizationService>();

        // Dependency injection for application services
        services.AddScoped<IAuthenticationService, AuthenticationService>();

        return services;
    }
}