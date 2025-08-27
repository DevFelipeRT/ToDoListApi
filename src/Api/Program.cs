using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.IdentityModel.Tokens;

using Application;
using Infrastructure;

using Api.Identity.Configuration;
using Api.Identity.Services;

using MediatR;
using Application.Abstractions.Messaging;
using Domain.Accounts.Events;
using Application.Notifications.Email;
using Application.Accounts.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// -----------------------------------------------------------------------------
// Configuration & strongly-typed options
// -----------------------------------------------------------------------------
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

// -----------------------------------------------------------------------------
// Application/Infrastructure layers
// Expectation: AddInfrastructureServices registers:
//   - ApplicationDbContext
//   - ASP.NET Identity (User/Role/Stores/TokenProviders)
//   - External infra services (email, persistence, etc.)
// -----------------------------------------------------------------------------
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

// -----------------------------------------------------------------------------
// JWT token service (application-side token issuance)
// -----------------------------------------------------------------------------
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

// -----------------------------------------------------------------------------
// Authentication (JWT Bearer)
// Align TokenValidationParameters with what JwtTokenService emits.
// MapInboundClaims=false to preserve standard JWT claim names (sub, email, etc.).
// -----------------------------------------------------------------------------
var jwtSection = builder.Configuration.GetSection("Jwt");
var isDevelopment = builder.Environment.IsDevelopment();

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata   = !isDevelopment; // true in production
        options.SaveToken              = false;
        options.MapInboundClaims       = false;          // keep original JWT claim names

        var secret = jwtSection["SecretKey"]
                     ?? throw new InvalidOperationException("JWT SecretKey not configured.");
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),

            ValidateIssuer   = true,
            ValidIssuer      = jwtSection["Issuer"],

            ValidateAudience = true,
            ValidAudience    = jwtSection["Audience"],

            ValidateLifetime = true,
            ClockSkew        = TimeSpan.Zero, // no default 5-minute skew

            // Match your JwtTokenService claim types:
            NameClaimType = JwtRegisteredClaimNames.UniqueName, // "unique_name"
            RoleClaimType = ClaimTypes.Role                      // "role" if you emit that explicitly
        };
    });

// -----------------------------------------------------------------------------
// Data protection (for email activation tokens, etc.)
// -----------------------------------------------------------------------------
var appName     = builder.Configuration["DP:APP_NAME"]      ?? "MyApp.Backend";
var keyRingPath = builder.Configuration["DP:KEY_RING_PATH"] ?? "/keys";

builder.Services
    .AddDataProtection()
    .SetApplicationName(appName)
    .PersistKeysToFileSystem(new DirectoryInfo(keyRingPath));

// -----------------------------------------------------------------------------
// Authorization (policies can be added here as needed)
// -----------------------------------------------------------------------------
builder.Services.AddAuthorization();

// -----------------------------------------------------------------------------
// MVC controllers
// -----------------------------------------------------------------------------
builder.Services.AddControllers();

// -----------------------------------------------------------------------------
// CORS (allow local React dev server)
// -----------------------------------------------------------------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

// -----------------------------------------------------------------------------
// Swagger/OpenAPI with HTTP Bearer auth
// -----------------------------------------------------------------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "ToDoApi", Version = "v1" });

    // Use HTTP Bearer scheme (not ApiKey) for JWT
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Type         = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme       = "bearer",
        BearerFormat = "JWT",
        Description  = "Enter: Bearer {your JWT token}"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id   = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// -----------------------------------------------------------------------------
// Development-time diagnostics (optional but helpful)
// -----------------------------------------------------------------------------
if (app.Environment.IsDevelopment())
{
    // Inspect how many MediatR handlers are registered for AccountRegistered
    using var scope = app.Services.CreateScope();
    var handlers = scope.ServiceProvider
        .GetServices<INotificationHandler<DomainEventNotification<AccountRegistered>>>();
    Console.WriteLine($"[BootCheck] AccountRegistered handlers: {handlers.Count()}");

    // Which IEmailSender implementation was resolved?
    var sender = scope.ServiceProvider.GetRequiredService<IEmailSender>();
    Console.WriteLine($"[BootCheck] IEmailSender: {sender.GetType().FullName}");
}

// -----------------------------------------------------------------------------
// HTTP pipeline
// -----------------------------------------------------------------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseCors("AllowReactApp");
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
