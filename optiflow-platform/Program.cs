using optiflow_platform.Inventory.Application.Internal.CommandServices;
using optiflow_platform.Inventory.Application.Internal.QueryServices;
using optiflow_platform.Inventory.Application.Services;
using optiflow_platform.Inventory.Domain.Repositories;
using optiflow_platform.Inventory.Infrastructure.Persistence.EFC.Repositories;
using optiflow_platform.LabAndOrders.Application.Internal.CommandServices;
using optiflow_platform.LabAndOrders.Application.Internal.QueryServices;
using optiflow_platform.LabAndOrders.Application.Services;
using optiflow_platform.LabAndOrders.Domain.Repositories;
using optiflow_platform.LabAndOrders.Infrastructure.Persistence.EFC.Repositories;
using optiflow_platform.Sales.Application.Internal.CommandServices;
using optiflow_platform.Sales.Application.Internal.QueryServices;
using optiflow_platform.Resources;
using optiflow_platform.Sales.Application.Services;
using optiflow_platform.Sales.Domain.Repositories;
using optiflow_platform.Sales.Infrastructure.Persistence.EFC.Repositories;
using optiflow_platform.Sales.Interfaces.Acl;
using optiflow_platform.Shared.Domain.Repositories;
using optiflow_platform.Shared.Interfaces.ASP.Configuration;
using optiflow_platform.Shared.Infrastructure.Persistence.EFC.Configuration;
using optiflow_platform.Shared.Infrastructure.Persistence.EFC.Repositories;
using optiflow_platform.Analytics.Application.Internal.QueryServices;
using optiflow_platform.Analytics.Application.Services;
using optiflow_platform.Analytics.Domain.Repositories;
using optiflow_platform.Analytics.Infrastructure.Persistence.EFC.Repositories;
using optiflow_platform.Clinical.Application.Internal.CommandServices;
using optiflow_platform.Clinical.Application.Internal.QueryServices;
using optiflow_platform.Clinical.Application.Services;
using optiflow_platform.Clinical.Domain.Repositories;
using optiflow_platform.Clinical.Infrastructure.Persistence.EFC.Repositories;
using optiflow_platform.Subscription.Application.Internal.CommandServices;
using optiflow_platform.Subscription.Application.Internal.QueryServices;
using optiflow_platform.Subscription.Application.Services;
using optiflow_platform.Subscription.Domain.Repositories;
using optiflow_platform.Subscription.Infrastructure.Persistence.EFC.Repositories;
using optiflow_platform.PatientCenter.Application.Internal.QueryServices;
using optiflow_platform.PatientCenter.Application.Services;
using optiflow_platform.PatientCenter.Domain.Repositories;
using optiflow_platform.PatientCenter.Infrastructure.Persistence.EFC.Repositories;
using optiflow_platform.PatientCenter.Application.Internal.CommandServices;

// Subscription aliases — disambiguate from Sales types with the same short name
using SubPaymentRepo      = optiflow_platform.Subscription.Domain.Repositories.IPaymentRepository;
using SubPaymentCmdSvc    = optiflow_platform.Subscription.Application.Services.IPaymentCommandService;
using SubPaymentQrySvc    = optiflow_platform.Subscription.Application.Services.IPaymentQueryService;
using SubPaymentCmdImpl   = optiflow_platform.Subscription.Application.Internal.CommandServices.PaymentCommandService;
using SubPaymentQryImpl   = optiflow_platform.Subscription.Application.Internal.QueryServices.PaymentQueryService;
using SubPaymentRepoImpl  = optiflow_platform.Subscription.Infrastructure.Persistence.EFC.Repositories.PaymentRepository;
// Sales aliases — needed now that both contexts are imported
using SalesPaymentRepo    = optiflow_platform.Sales.Domain.Repositories.IPaymentRepository;
using SalesPaymentRepoImpl= optiflow_platform.Sales.Infrastructure.Persistence.EFC.Repositories.PaymentRepository;
using SalesPaymentCmdSvc  = optiflow_platform.Sales.Application.Services.IPaymentCommandService;
using SalesPaymentQrySvc  = optiflow_platform.Sales.Application.Services.IPaymentQueryService;
using SalesPaymentCmdImpl = optiflow_platform.Sales.Application.Internal.CommandServices.PaymentCommandService;
using SalesPaymentQryImpl = optiflow_platform.Sales.Application.Internal.QueryServices.PaymentQueryService;

using Cortex.Mediator.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

var builder = WebApplication.CreateBuilder(args);

// Configure lowercase URLs
builder.Services.AddRouting(options => options.LowercaseUrls = true);

// Localization Configuration
builder.Services.AddLocalization();

// Configure Kebab Case Route Naming Convention
builder.Services.AddControllers(options => options.Conventions.Add(new KebabCaseRouteNamingConvention()))
    .AddDataAnnotationsLocalization();

// Register RFC 7807 ProblemDetails payloads for centralized exception handling.
builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        if (context.ProblemDetails.Status is null or >= 500)
        {
            var localizer = context.HttpContext.RequestServices.GetRequiredService<IStringLocalizer<SharedResource>>();
            context.ProblemDetails.Title ??= localizer["UnexpectedServerError"].Value;
            context.ProblemDetails.Detail ??= localizer["UnexpectedErrorProcessingRequest"].Value;
        }
    };
});

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
    if (string.IsNullOrWhiteSpace(connectionString) || connectionString.Contains('%'))
        throw new InvalidOperationException($"Database connection string contains unresolved environment variables: {connectionString}");

    options.UseMySQL(connectionString)
        .UseLoggerFactory(serviceProvider.GetRequiredService<ILoggerFactory>())
        .EnableDetailedErrors();

    if (builder.Environment.IsDevelopment())
        options.EnableSensitiveDataLogging();
});

// Cortex Mediator — scans this assembly for all IEventHandler implementations
builder.Services.AddCortexMediator([typeof(Program)]);

// Shared Bounded Context Injection
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Lab and Orders Bounded Context Injection
builder.Services.AddScoped<IWorkOrderRepository, WorkOrderRepository>();
builder.Services.AddScoped<ILaboratoryRepository, LaboratoryRepository>();
builder.Services.AddScoped<IWorkOrderCommandService, WorkOrderCommandService>();
builder.Services.AddScoped<IWorkOrderQueryService, WorkOrderQueryService>();
builder.Services.AddScoped<ILaboratoryQueryService, LaboratoryQueryService>();
builder.Services.AddScoped<ILaboratoryCommandService, LaboratoryCommandService>();

// Analytics Bounded Context Injection
builder.Services.AddScoped<IAnalyticsReportRepository, AnalyticsReportRepository>();
builder.Services.AddScoped<IStaffMetricRepository, StaffMetricRepository>();
builder.Services.AddScoped<IAnalyticsReportQueryService, AnalyticsReportQueryService>();
builder.Services.AddScoped<IStaffMetricQueryService, StaffMetricQueryService>();

// Clinical Bounded Context Injection
builder.Services.AddScoped<IPatientRepository, PatientRepository>();
builder.Services.AddScoped<IClinicalRecordRepository, ClinicalRecordRepository>();
builder.Services.AddScoped<IPrescriptionRepository, PrescriptionRepository>();
builder.Services.AddScoped<IPatientCommandService, PatientCommandService>();
builder.Services.AddScoped<IPatientQueryService, PatientQueryService>();
builder.Services.AddScoped<IClinicalRecordQueryService, ClinicalRecordQueryService>();
builder.Services.AddScoped<IPrescriptionCommandService, PrescriptionCommandService>();
builder.Services.AddScoped<IPrescriptionQueryService, PrescriptionQueryService>();
// Sales Bounded Context Injection
builder.Services.AddScoped<ILabAndOrdersContextFacade, LabAndOrdersContextFacade>();
builder.Services.AddScoped<ISaleRepository, SaleRepository>();
builder.Services.AddScoped<SalesPaymentRepo, SalesPaymentRepoImpl>();
builder.Services.AddScoped<ISaleCommandService, SaleCommandService>();
builder.Services.AddScoped<ISaleQueryService, SaleQueryService>();
builder.Services.AddScoped<SalesPaymentCmdSvc, SalesPaymentCmdImpl>();
builder.Services.AddScoped<SalesPaymentQrySvc, SalesPaymentQryImpl>();

// Subscription Bounded Context Injection
builder.Services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
builder.Services.AddScoped<SubPaymentRepo, SubPaymentRepoImpl>();
builder.Services.AddScoped<IBillingRepository, BillingRepository>();
builder.Services.AddScoped<IPlanRepository, PlanRepository>();
builder.Services.AddScoped<ISubscriptionCommandService, SubscriptionCommandService>();
builder.Services.AddScoped<SubPaymentCmdSvc, SubPaymentCmdImpl>();
builder.Services.AddScoped<IBillingCommandService, BillingCommandService>();
builder.Services.AddScoped<IPlanCommandService, PlanCommandService>();
builder.Services.AddScoped<ISubscriptionQueryService, SubscriptionQueryService>();
builder.Services.AddScoped<SubPaymentQrySvc, SubPaymentQryImpl>();
builder.Services.AddScoped<IBillingQueryService, BillingQueryService>();
builder.Services.AddScoped<IPlanQueryService, PlanQueryService>();

// Inventory Bounded Context Injection
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IStockAuditLogRepository, StockAuditLogRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
builder.Services.AddScoped<IProductCommandService, ProductCommandService>();
builder.Services.AddScoped<IProductQueryService, ProductQueryService>();
builder.Services.AddScoped<ICategoryQueryService, CategoryQueryService>();
builder.Services.AddScoped<ISupplierCommandService, SupplierCommandService>();
builder.Services.AddScoped<ISupplierQueryService, SupplierQueryService>();
builder.Services.AddScoped<IStockAuditLogQueryService, StockAuditLogQueryService>();

// PatientCenter Bounded Context Injection
builder.Services.AddScoped<IPatientNotificationRepository, PatientNotificationRepository>();
builder.Services.AddScoped<ILensMaterialRepository, LensMaterialRepository>();
builder.Services.AddScoped<IPatientNotificationQueryService, PatientNotificationQueryService>();
builder.Services.AddScoped<ILensMaterialQueryService, LensMaterialQueryService>();
builder.Services.AddScoped<IPatientNotificationCommandService, PatientNotificationCommandService>();
var app = builder.Build();

// Apply pending EF Core migrations on startup
using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<AppDbContext>>();
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        context.Database.Migrate();
    }
    catch (Exception ex)
    {
        logger.LogCritical(ex, "Database migration failed on startup. App will continue but DB may be unavailable.");
    }
}

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

app.UseSwagger();
app.UseSwaggerUI();

// Localization Configuration
string[] supportedCultures = ["en", "en-US", "es", "es-PE"];
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture(supportedCultures[0])
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);
localizationOptions.ApplyCurrentCultureToResponseHeaders = true;
app.UseRequestLocalization(localizationOptions);

app.UseCors();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
