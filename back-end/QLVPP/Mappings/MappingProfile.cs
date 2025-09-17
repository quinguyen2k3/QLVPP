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

            CreateMap<EmployeeReq, Employee>();
            CreateMap<Employee, EmployeeRes>();

            CreateMap<Department, DepartmentRes>();
            CreateMap<DepartmentReq, Department>();

            CreateMap<Unit, UnitRes>();
            CreateMap<UnitReq, Unit>();

            CreateMap<Supplier, SupplierRes>();
            CreateMap<SupplierReq, Supplier>();
        }
    }
}
