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

            CreateMap<RequisitionReq, Requisition>()
                .ForMember(dest => dest.RequisitionDetails,
                           opt => opt.MapFrom(src => src.Items));
            CreateMap<RequisitionItemReq, RequisitionDetail>()
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.Purpose, opt => opt.MapFrom(src => src.Purpose));
            CreateMap<Requisition, RequisitionRes>()
               .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee.Name))
               .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.RequisitionDetails));
            CreateMap<RequisitionDetail, RequisitionItemRes>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.UnitName, opt => opt.MapFrom(src => src.Product.Unit.Name))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.Purpose, opt => opt.MapFrom(src => src.Purpose));
        }
    }
}
