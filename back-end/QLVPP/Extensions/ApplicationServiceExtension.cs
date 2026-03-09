using QLVPP.Filters;
using QLVPP.Repositories;
using QLVPP.Repositories.Implementations;
using QLVPP.Services;
using QLVPP.Services.Implementations;

namespace QLVPP.Extensions
{
    public static class ApplicationServiceExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddControllers(options =>
            {
                options.Filters.Add<DynamicAuthorizeFilter>();
            });

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<IDepartmentService, DepartmentService>();
            services.AddScoped<IUnitService, UnitService>();
            services.AddScoped<ISupplierService, SupplierService>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IWarehouseService, WarehouseService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IStockInService, StockInService>();
            services.AddScoped<IInvalidTokenService, InvalidTokenService>();
            services.AddScoped<IStockOutService, StockOutService>();
            services.AddScoped<IInventoryService, InventoryService>();
            services.AddScoped<IReportService, ReportService>();
            services.AddScoped<IOnlineUserService, OnlineUserService>();
            services.AddScoped<IStockTakeService, StockTakeService>();
            services.AddScoped<IFileUploadService, FileUploadService>();
            services.AddScoped<IPositionService, PositionService>();
            services.AddScoped<IStockTakeService, StockTakeService>();
            services.AddScoped<IMaterialRequestService, MaterialRequestService>();

            services.AddSingleton<ICacheService, CacheService>();

            services.AddAutoMapper(typeof(Program));

            services.AddHttpContextAccessor();

            return services;
        }
    }
}
