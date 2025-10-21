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

var builder = WebApplication.CreateBuilder(args);

// ================== Config & Settings ==================
var configuration = builder.Configuration;

// JWT Settings
var jwtSettings = configuration.GetSection("JwtSec");
var keyString = jwtSettings["Key"] ?? throw new ArgumentNullException("JwtSettings:Key is missing");
var key = Encoding.UTF8.GetBytes(keyString);

// ================== Services ==================

// --- DbContext ---
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
);

// --- Repositories / UnitOfWork ---
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// --- Application Services ---
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

// --- AutoMapper ---
builder.Services.AddAutoMapper(typeof(Program));

// --- HTTP Context ---
builder.Services.AddHttpContextAccessor();

// --- JWT Authentication ---
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
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ClockSkew = TimeSpan.Zero,
        };
    });

// --- Rate Limiting ---
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(configuration.GetSection("IpRateLimiting"));
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

// --- Swagger ---
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
                new string[] { }
            },
        }
    );
});

// --- Controllers ---
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

// --- Authentication & Authorization ---
app.UseAuthentication();
app.UseAuthorization();

// --- Rate Limiting Middleware ---
app.UseIpRateLimiting();

// --- Custom Middlewares ---
app.UseMiddleware<AccountAccessMiddleware>();
app.UseMiddleware<RevokedTokenMiddleware>();

// --- Map Controllers ---
app.MapControllers();

app.Run();