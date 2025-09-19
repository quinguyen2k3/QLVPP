using AutoMapper;
using QLVPP.DTOs.Request;
using QLVPP.DTOs.Response;
using QLVPP.Models;

namespace QLVPP.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CategoryReq, Category>();
            CreateMap<Category, CategoryRes>();

            CreateMap<EmployeeReq, Employee>()
                    .ForMember(dest => dest.Password, opt => opt.Ignore());
            CreateMap<Employee, EmployeeRes>()
                 .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department.Name));

            CreateMap<Department, DepartmentRes>();
            CreateMap<DepartmentReq, Department>();

            CreateMap<Unit, UnitRes>();
            CreateMap<UnitReq, Unit>();

            CreateMap<Supplier, SupplierRes>();
            CreateMap<SupplierReq, Supplier>();

            CreateMap<Warehouse,  WarehouseRes>();
            CreateMap<WarehouseReq,  Warehouse>();
        }
    }
}
