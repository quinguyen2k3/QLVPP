using System.Text;
using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using QLVPP.Data;
using QLVPP.Middlewares;
using QLVPP.Repositories;
using QLVPP.Repositories.Implementations;
using QLVPP.Services;
using QLVPP.Services.Implementations;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// ================== Database Connection ==================
var connectionString =
    configuration.GetConnectionString("DefaultConnection")
    ?? throw new ArgumentNullException("Connection string is missing in appsettings.json");

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

// ================== Redis ==================
var redisHost = configuration["Redis:Host"] ?? "localhost";
var redisPort = configuration["Redis:Port"] ?? "6379";
var redisPassword = configuration["Redis:Password"] ?? "";

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configOptions = new ConfigurationOptions
    {
        EndPoints = { $"{redisHost}:{redisPort}" },
        Password = redisPassword,
        AbortOnConnectFail = false,
    };
    return ConnectionMultiplexer.Connect(configOptions);
});

// ================== JWT ==================
var jwtKey =
    configuration["Jwt:Key"]
    ?? throw new ArgumentNullException("Jwt:Key is missing in appsettings.json");
var jwtIssuer =
    configuration["Jwt:Issuer"]
    ?? throw new ArgumentNullException("Jwt:Issuer is missing in appsettings.json");
var jwtAudience =
    configuration["Jwt:Audience"]
    ?? throw new ArgumentNullException("Jwt:Audience is missing in appsettings.json");

var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

// ================== Routing ==================
builder.Services.AddRouting(options => options.LowercaseUrls = true);

// ================== Dependency Injection ==================
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IUnitService, UnitService>();
builder.Services.AddScoped<ISupplierService, SupplierService>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IWarehouseService, WarehouseService>();
builder.Services.AddScoped<IRequisitionService, RequisitionService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IInvalidTokenService, InvalidTokenService>();
builder.Services.AddScoped<IDeliveryService, DeliveryService>();
builder.Services.AddScoped<IReturnService, ReturnService>();
builder.Services.AddScoped<IInventorySnapshotService, InventorySnapshotService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IOnlineUserService, OnlineUserService>();
builder.Services.AddSingleton<ICacheService, CacheService>();

builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddHttpContextAccessor();

// ================== Authentication ==================
builder
    .Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = key,
            ClockSkew = TimeSpan.Zero,
        };
    });

// ================== Rate Limiting ==================
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(configuration.GetSection("IpRateLimiting"));
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

// ================== Swagger ==================
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "QLVPP API", Version = "v1" });

    c.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Nhập JWT token vào đây: Bearer {your token}",
        }
    );

    c.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer",
                    },
                },
                Array.Empty<string>()
            },
        }
    );
});

builder.Services.AddControllers();

var app = builder.Build();

// ================== Seed Data ==================
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<AppDbContext>>();
    await AppDataSeed.SeedAsync(context, logger);
}

// ================== Middleware Pipeline ==================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseIpRateLimiting();
app.UseMiddleware<AccountAccessMiddleware>();
app.UseMiddleware<RevokedTokenMiddleware>();
app.MapControllers();

app.Run();
