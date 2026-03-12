using AutoMapper;
using QLVPP.Mappings; // namespace chứa các profile của bạn

namespace QLVPP.Extensions
{
    public static class AutoMapperServiceExtensions
    {
        public static IServiceCollection AddCustomMappings(this IServiceCollection services)
        {
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<DepartmentMappingProfile>();
                cfg.AddProfile<EmployeeMappingProfile>();
                cfg.AddProfile<ProductMappingProfile>();
                cfg.AddProfile<StockInMappingProfile>();
                cfg.AddProfile<StockOutMappingProfile>();
                cfg.AddProfile<StockTakeMappingProfile>();
                cfg.AddProfile<CategoryMappingProfile>();
                cfg.AddProfile<UnitMappingProfile>();
                cfg.AddProfile<WarehouseMappingProfile>();
                cfg.AddProfile<SupplierMappingProfile>();
                cfg.AddProfile<InventorySnapshotMappingProfile>();
                cfg.AddProfile<PositionMappingProfile>();
                cfg.AddProfile<MaterialRequestMappingProfile>();
                cfg.AddProfile<RoleMappingProfile>();
            });

            IMapper mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);

            return services;
        }
    }
}
