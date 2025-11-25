using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
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
                cfg.AddProfile<RequisitionMappingProfile>();
                cfg.AddProfile<ReturnMappingProfile>();
                cfg.AddProfile<StockInMappingProfile>();
                cfg.AddProfile<StockOutMappingProfile>();
                cfg.AddProfile<StockTakeMappingProfile>();
                cfg.AddProfile<TransferMappingProfile>();
                cfg.AddProfile<ApprovalFlowMappingProfile>();
            });

            IMapper mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);

            return services;
        }
    }
}
