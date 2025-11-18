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
            services.AddScoped<IRequisitionService, RequisitionService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IStockInService, StockInService>();
            services.AddScoped<IInvalidTokenService, InvalidTokenService>();
            services.AddScoped<IStockOutService, StockOutService>();
            services.AddScoped<IReturnService, ReturnService>();
            services.AddScoped<IInventorySnapshotService, InventorySnapshotService>();
            services.AddScoped<IReportService, ReportService>();
            services.AddScoped<IOnlineUserService, OnlineUserService>();
            services.AddScoped<ITransferService, TransferService>();
            services.AddScoped<IStockTakeService, StockTakeService>();

            services.AddSingleton<ICacheService, CacheService>();

            services.AddAutoMapper(typeof(Program));

            services.AddHttpContextAccessor();

            return services;
        }
    }
}
