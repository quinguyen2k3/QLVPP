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
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IInvalidTokenService, InvalidTokenService>();
            services.AddScoped<IDeliveryService, DeliveryService>();
            services.AddScoped<IReturnService, ReturnService>();
            services.AddScoped<IInventorySnapshotService, InventorySnapshotService>();
            services.AddScoped<IReportService, ReportService>();
            services.AddScoped<IOnlineUserService, OnlineUserService>();

            services.AddSingleton<ICacheService, CacheService>();

            services.AddAutoMapper(typeof(Program));

            services.AddHttpContextAccessor();

            return services;
        }
    }
}
