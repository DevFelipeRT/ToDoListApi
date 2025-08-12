using System.Reflection;
using Application.Accounts.Services;
using Application.Accounts.Services.Interfaces;
using Application.Lists.Services;
using Domain.Accounts.Services;
using Domain.Accounts.Services.Interfaces;
using Domain.Lists.Services;
using Domain.Lists.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

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
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        // Dependency injection for domain services
        services.AddScoped<IAccountLockoutPolicy, AccountLockoutPolicy>();
        services.AddScoped<IAccountUniquenessChecker, AccountUniquenessChecker>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IPasswordPolicyValidator, PasswordPolicyValidator>();
        services.AddScoped<IReminderSchedulerService, ReminderSchedulerService>();
        services.AddScoped<IToDoListItemTransferService, ToDoListItemTransferService>();
        services.AddScoped<IToDoListTitleUniquenessChecker, ToDoListTitleUniquenessChecker>();
        services.AddScoped<IAuthorizationService, AuthorizationService>();

        // Dependency injection for application services
        services.AddScoped<IAuthenticationService, AuthenticationService>();

        return services;
    }
}