using ToDoList.Application;
using ToDoList.Infrastructure;
using ToDoList.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services from our other layers
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

// Add services required for a controller-based API
builder.Services.AddControllers();

// Add services for API documentation using Swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Seed database with initial data
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await ApplicationDbContextSeed.SeedAsync(db);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();