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
using optiflow_platform.Shared.Infrastructure.Security;
using optiflow_platform.Shared.Interfaces.ASP.Configuration;
using optiflow_platform.Shared.Infrastructure.Persistence.EFC.Configuration;
using optiflow_platform.Shared.Infrastructure.Persistence.EFC.Repositories;
using optiflow_platform.Shared.Application.Services;
using optiflow_platform.Shared.Application.Internal.CommandServices;
using optiflow_platform.Shared.Application.Internal.QueryServices;
using optiflow_platform.Analytics.Application.Internal.QueryServices;
using optiflow_platform.Analytics.Application.Services;
using optiflow_platform.Analytics.Domain.Repositories;
using optiflow_platform.Analytics.Infrastructure.Persistence.EFC.Repositories;
using optiflow_platform.Analytics.Interfaces.Acl;
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
using Stripe;
using optiflow_platform.Subscription.Application.Internal.OutboundServices.Stripe;
using optiflow_platform.Subscription.Infrastructure.Stripe.Services;

// IAM Bounded Context Imports
using optiflow_platform.IAM.Application.CommandServices;
using optiflow_platform.IAM.Application.QueryServices;
using optiflow_platform.IAM.Application.Internal.CommandServices;
using optiflow_platform.IAM.Application.Internal.QueryServices;
using optiflow_platform.IAM.Application.Internal.OutboundServices.Email;
using optiflow_platform.IAM.Application.Internal.OutboundServices.Hashing;
using optiflow_platform.IAM.Application.Internal.OutboundServices.Tokens;
using optiflow_platform.IAM.Domain.Repositories;
using optiflow_platform.IAM.Infrastructure.Email.Smtp;
using optiflow_platform.IAM.Infrastructure.Hashing.BCrypt;
using optiflow_platform.IAM.Infrastructure.Tokens.Jwt.Services;
using optiflow_platform.IAM.Infrastructure.Tokens.Jwt.Configuration;
using optiflow_platform.IAM.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using optiflow_platform.IAM.Infrastructure.Pipeline.Middleware.Extensions;
using optiflow_platform.Shared.Interfaces.Rest.ProblemDetails;
using Microsoft.OpenApi;

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
    .AddDataAnnotationsLocalization()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter()));

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
builder.Services.AddSwaggerGen(options =>
{
    options.EnableAnnotations();
    
    // Add Bearer Security Definition
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token in format 'Bearer {your_token}'",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    
    // Add Bearer Security Requirement
    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
    });
});

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
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserContext, CurrentUserContext>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ISystemNotificationRepository, SystemNotificationRepository>();
builder.Services.AddScoped<ISystemNotificationCommandService, SystemNotificationCommandService>();
builder.Services.AddScoped<ISystemNotificationQueryService, SystemNotificationQueryService>();

// Lab and Orders Bounded Context Injection
builder.Services.AddScoped<IWorkOrderRepository, WorkOrderRepository>();
builder.Services.AddScoped<ILaboratoryRepository, LaboratoryRepository>();
builder.Services.AddScoped<IWorkOrderCommandService, WorkOrderCommandService>();
builder.Services.AddScoped<IWorkOrderQueryService, WorkOrderQueryService>();
builder.Services.AddScoped<ILaboratoryQueryService, LaboratoryQueryService>();
builder.Services.AddScoped<ILaboratoryCommandService, LaboratoryCommandService>();

// Analytics Bounded Context Injection
builder.Services.AddScoped<IStaffMetricRepository, StaffMetricRepository>();
builder.Services.AddScoped<ISalesContextFacade, SalesContextFacade>();
builder.Services.AddScoped<ILabOrdersContextFacade, LabOrdersContextFacade>();
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
builder.Services.AddScoped<ISaleItemRepository, SaleItemRepository>();
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
builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
builder.Services.AddScoped<IProductCommandService, ProductCommandService>();
builder.Services.AddScoped<IProductQueryService, ProductQueryService>();
builder.Services.AddScoped<ISupplierCommandService, SupplierCommandService>();
builder.Services.AddScoped<ISupplierQueryService, SupplierQueryService>();
builder.Services.AddScoped<IStockAuditLogQueryService, StockAuditLogQueryService>();

// PatientCenter Bounded Context Injection
builder.Services.AddScoped<IPatientNotificationRepository, PatientNotificationRepository>();
builder.Services.AddScoped<ILensMaterialRepository, LensMaterialRepository>();
builder.Services.AddScoped<IPatientNotificationQueryService, PatientNotificationQueryService>();
builder.Services.AddScoped<ILensMaterialQueryService, LensMaterialQueryService>();
builder.Services.AddScoped<IPatientNotificationCommandService, PatientNotificationCommandService>();

// Staff Bounded Context Injection
builder.Services.AddScoped<optiflow_platform.Staff.Domain.Repositories.IStaffRepository, optiflow_platform.Staff.Infrastructure.Persistence.EFC.Repositories.StaffRepository>();
builder.Services.AddScoped<optiflow_platform.Staff.Application.Services.IStaffCommandService, optiflow_platform.Staff.Application.Internal.CommandServices.StaffCommandService>();
builder.Services.AddScoped<optiflow_platform.Staff.Application.Services.IStaffQueryService, optiflow_platform.Staff.Application.Internal.QueryServices.StaffQueryService>();
builder.Services.AddScoped<optiflow_platform.Staff.Domain.Repositories.IRoleRepository, optiflow_platform.Staff.Infrastructure.Persistence.EFC.Repositories.RoleRepository>();
builder.Services.AddScoped<optiflow_platform.Staff.Application.Services.IRoleCommandService, optiflow_platform.Staff.Application.Internal.CommandServices.RoleCommandService>();
builder.Services.AddScoped<optiflow_platform.Staff.Application.Services.IRoleQueryService, optiflow_platform.Staff.Application.Internal.QueryServices.RoleQueryService>();

// IAM Bounded Context Injection
builder.Services.AddScoped<ProblemDetailsFactory>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPasswordRecoveryTokenRepository, PasswordRecoveryTokenRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IUserCommandService, UserCommandService>();
builder.Services.AddScoped<IAccountCommandService, AccountCommandService>();
builder.Services.AddScoped<IPasswordRecoveryCommandService, PasswordRecoveryCommandService>();
builder.Services.AddScoped<IUserQueryService, UserQueryService>();
builder.Services.AddScoped<IAccountQueryService, AccountQueryService>();
builder.Services.AddScoped<IHashingService, BCryptHashingService>();
builder.Services.AddScoped<ITokenService, optiflow_platform.IAM.Infrastructure.Tokens.Jwt.Services.TokenService>();builder.Services.AddScoped<IEmailService, SmtpEmailService>();
builder.Services.AddScoped<optiflow_platform.IAM.Application.Internal.OutboundServices.Patients.IPatientDirectoryService, optiflow_platform.IAM.Infrastructure.Acl.PatientDirectoryService>();
builder.Services.AddScoped<optiflow_platform.IAM.Application.ACL.IClientAccountService, optiflow_platform.IAM.Application.Internal.ACL.ClientAccountService>();
// IAM Token Settings Configuration
builder.Services.Configure<TokenSettings>(builder.Configuration.GetSection("AppSettings:JwtSettings"));
builder.Services.PostConfigure<TokenSettings>(settings =>
{
    // Resolve %VAR% placeholders from the environment like the DB connection string above, then fail fast
    // on an unresolved placeholder or a key too short for HMAC-SHA256 (needs 128 bits / 16 bytes) instead
    // of surfacing the cryptic IDX10653 at token-signing time.
    settings.Secret = Environment.ExpandEnvironmentVariables(settings.Secret ?? string.Empty);
    if (string.IsNullOrWhiteSpace(settings.Secret) || settings.Secret.Contains('%') || settings.Secret.Length < 16)
        throw new InvalidOperationException(
            "JWT secret is missing, unresolved, or shorter than 16 characters (128 bits required for HMAC-SHA256). " +
            "Set JWT_SECRET (or AppSettings:JwtSettings:Secret) to a long random value.");
});


// Cargar config local (en .gitignore)
builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);

// Resolve %VAR% placeholders in the frontend URL from the environment, like the DB connection string,
// so post-payment Stripe redirects reach the real site instead of a literal "%FRONTEND_URL%".
// Normalize to the site origin (scheme://host[:port]) so redirects like "{frontend}/payment-success"
// stay correct even when FRONTEND_URL is misconfigured with a path suffix (e.g. ".../select-plan").
var frontendUrl = builder.Configuration["AppSettings:FrontendUrl"];
if (!string.IsNullOrWhiteSpace(frontendUrl))
{
    var expandedFrontendUrl = Environment.ExpandEnvironmentVariables(frontendUrl);
    if (Uri.TryCreate(expandedFrontendUrl, UriKind.Absolute, out var frontendUri))
        expandedFrontendUrl = frontendUri.GetLeftPart(UriPartial.Authority);
    builder.Configuration["AppSettings:FrontendUrl"] = expandedFrontendUrl;
}

// Configurar Stripe API key
StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

// Registrar servicio
builder.Services.AddScoped<IStripeCheckoutService, StripeCheckoutService>();

var app = builder.Build();

// Apply pending EF Core migrations on startup
using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<AppDbContext>>();
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        context.Database.Migrate();

        // Ensure the default subscription plan catalog exists (and stays free of duplicates),
        // otherwise "select plan" has nothing to show or shows duplicated plans.
        var planCommandService = scope.ServiceProvider.GetRequiredService<IPlanCommandService>();
        var planRepository = scope.ServiceProvider.GetRequiredService<optiflow_platform.Subscription.Domain.Repositories.IPlanRepository>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<optiflow_platform.Shared.Domain.Repositories.IUnitOfWork>();
        await optiflow_platform.Subscription.Infrastructure.Seeding.PlanSeeder.SeedAsync(planCommandService, planRepository, unitOfWork, logger);
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

app.UseRequestAuthorization();

app.UseAuthorization();

app.MapControllers();

app.Run();