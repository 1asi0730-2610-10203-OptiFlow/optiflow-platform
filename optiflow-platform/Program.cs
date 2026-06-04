using optiflow_platform.LabAndOrders.Application.Internal.CommandServices;
using optiflow_platform.LabAndOrders.Application.Internal.QueryServices;
using optiflow_platform.LabAndOrders.Application.Services;
using optiflow_platform.LabAndOrders.Domain.Repositories;
using optiflow_platform.LabAndOrders.Infrastructure.Persistence.EFC.Repositories;
using optiflow_platform.Shared.Domain.Repositories;
using optiflow_platform.Shared.Infrastructure.Interfaces.ASP.Configuration;
using optiflow_platform.Shared.Infrastructure.Persistence.EFC.Configuration;
using optiflow_platform.Shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configure lowercase URLs
builder.Services.AddRouting(options => options.LowercaseUrls = true);

// Configure Kebab Case Route Naming Convention
builder.Services.AddControllers(options => options.Conventions.Add(new KebabCaseRouteNamingConvention()));

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => options.EnableAnnotations());

// Configure CORS for frontend
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

// Configure Database Context
builder.Services.AddDbContext<AppDbContext>((serviceProvider, options) =>
{
    var connectionStringTemplate = builder.Configuration.GetConnectionString("DefaultConnection");
    if (string.IsNullOrWhiteSpace(connectionStringTemplate))
        throw new InvalidOperationException("Database connection string is not set in the configuration.");

    var connectionString = Environment.ExpandEnvironmentVariables(connectionStringTemplate);
    if (string.IsNullOrWhiteSpace(connectionString))
        throw new InvalidOperationException("Database connection string is not set in the configuration.");

    options.UseMySQL(connectionString)
        .UseLoggerFactory(serviceProvider.GetRequiredService<ILoggerFactory>())
        .EnableDetailedErrors();

    if (builder.Environment.IsDevelopment())
        options.EnableSensitiveDataLogging();
});

// Shared Bounded Context Injection
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Lab and Orders Bounded Context Injection
builder.Services.AddScoped<IWorkOrderRepository, WorkOrderRepository>();
builder.Services.AddScoped<ILaboratoryRepository, LaboratoryRepository>();
builder.Services.AddScoped<IWorkOrderCommandService, WorkOrderCommandService>();
builder.Services.AddScoped<IWorkOrderQueryService, WorkOrderQueryService>();
builder.Services.AddScoped<ILaboratoryQueryService, LaboratoryQueryService>();

var app = builder.Build();

// Apply pending EF Core migrations on startup
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.Migrate();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
