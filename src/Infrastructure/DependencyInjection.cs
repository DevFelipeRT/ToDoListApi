using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Domain.Lists.Repositories;
using Domain.Accounts.Repositories;
using Application.Common.Interfaces;
using Application.Notifications.Email;
using Infrastructure.Persistence;
using Infrastructure.Email;
using Infrastructure.Links;
using Application.Abstractions.Persistence;
using Application.Abstractions.Messaging;
using Infrastructure.Messaging;
using Infrastructure.IdentityAccess;
using Infrastructure.IdentityAccess.Services;
using Infrastructure.Persistence.Lists;
using Infrastructure.Persistence.Accounts;
using Application.IdentityAccess;
using Application.IdentityAccess.Services;
using Application.Abstractions.Links;
using Application.IdentityAccess.Links;

namespace Infrastructure;

/// <summary>
/// Registers infrastructure services: persistence, Identity, messaging, email, and link utilities.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds infrastructure services to the DI container.
    /// </summary>
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // DbContext
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sql => sql.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        // ASP.NET Core Identity
        services
            .AddIdentityCore<ApplicationUser>(options =>
            {
                options.User.RequireUniqueEmail = true;

                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredUniqueChars = 4;

                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                options.Lockout.AllowedForNewUsers = true;
            })
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders();

        // Identity token lifetime
        services.Configure<DataProtectionTokenProviderOptions>(o =>
        {
            o.TokenLifespan = TimeSpan.FromHours(24);
        });

        // Database utilities
        services
            .AddScoped<IDatabaseSeeder, ApplicationDbContextSeed>()
            .AddScoped<IUnitOfWork, EfUnitOfWork>();

        // Repositories
        services
            .AddScoped<IToDoListRepository, ToDoListRepository>()
            .AddScoped<IAccountRepository, AccountRepository>();

        // Identity workflows
        services
            .AddScoped<IIdentityGateway, IdentityService>()
            .AddScoped<IEmailActivationService, EmailActivationService>()
            .AddScoped<IPasswordResetService, PasswordResetService>();

        // Domain events
        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

        // Email sender (disk outbox)
        var outboxDir   = Environment.GetEnvironmentVariable("EMAIL_OUTBOX_DIR")   ?? "/data/outbox";
        var defaultFrom = Environment.GetEnvironmentVariable("EMAIL_DEFAULT_FROM") ?? "no-reply@todoapp.local";
        services.AddSingleton<IEmailSender>(_ => new DiskEmailSender(outputFolder: outboxDir, defaultFrom: defaultFrom));

        // Link utilities
        services.AddSingleton<ILinkBuilder, LinkBuilder>();
        services.AddSingleton<IUrlCrypto, UrlCrypto>(); // swap for DataProtectionUrlCrypto if applicable

        // Options binding (concrete types)
        services.Configure<ActivationLinkOptions>(configuration.GetSection("Links:Activation"));
        services.Configure<ResetPasswordLinkOptions>(configuration.GetSection("Links:PasswordReset"));

        // URL services resolved by constructor (no factory)
        services.AddScoped<IActivationUrlService, ActivationUrlService>();
        // services.AddScoped<IResetPasswordUrlService, ResetPasswordUrlService>(); // when implemented

        return services;
    }
}
