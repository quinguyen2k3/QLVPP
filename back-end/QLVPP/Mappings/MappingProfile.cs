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
                .ForMember(
                    dest => dest.DepartmentName,
                    opt => opt.MapFrom(src => src.Department.Name)
                );

            CreateMap<Department, DepartmentRes>();
            CreateMap<DepartmentReq, Department>();

            CreateMap<Unit, UnitRes>();
            CreateMap<UnitReq, Unit>();

            CreateMap<Supplier, SupplierRes>();
            CreateMap<SupplierReq, Supplier>();

            CreateMap<Warehouse, WarehouseRes>();
            CreateMap<WarehouseReq, Warehouse>();

            CreateMap<RequisitionReq, Requisition>()
                .ForMember(dest => dest.RequisitionDetails, opt => opt.MapFrom(src => src.Items));
            CreateMap<RequisitionItemReq, RequisitionDetail>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
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

            CreateMap<Product, ProductRes>()
                .ForMember(
                    dest => dest.WarehouseId,
                    opt => opt.MapFrom(src => src.Inventory.WarehouseId)
                )
                .ForMember(
                    dest => dest.Quantity,
                    opt => opt.MapFrom(src => src.Inventory.Quantity)
                );
            CreateMap<ProductReq, Product>();

            CreateMap<OrderReq, Order>()
                .ForMember(dest => dest.OrderDetails, opt => opt.MapFrom(src => src.Items))
                .ForMember(dest => dest.Status, opt => opt.Ignore());
            CreateMap<OrderItemReq, OrderDetail>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice))
                .ForMember(dest => dest.Received, opt => opt.MapFrom(src => src.Received));

            CreateMap<Order, OrderRes>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.OrderDetails));
            CreateMap<OrderDetail, OrderItemRes>()
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice))
                .ForMember(dest => dest.Received, opt => opt.MapFrom(src => src.Received));

            CreateMap<InvalidToken, InvalidTokenRes>();

            CreateMap<DeliveryReq, Delivery>()
                .ForMember(dest => dest.DeliveryDetails, opt => opt.MapFrom(src => src.Items));
            CreateMap<DeliveryItemReq, DeliveryDetail>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity));
            CreateMap<Delivery, DeliveryRes>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.DeliveryDetails));
            CreateMap<DeliveryDetail, DeliveryItemRes>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.UnitName, opt => opt.MapFrom(src => src.Product.Unit.Name))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity));

            CreateMap<ReturnReq, Return>()
                .ForMember(dest => dest.ReturnDetails, opt => opt.MapFrom(src => src.Items))
                .ForMember(dest => dest.Status, opt => opt.Ignore());
            CreateMap<ReturnItemReq, ReturnDetail>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(
                    dest => dest.DamagedQuantity,
                    opt => opt.MapFrom(src => src.DamagedQuantity)
                )
                .ForMember(
                    dest => dest.ReturnedQuantity,
                    opt => opt.MapFrom(src => src.ReturnedQuantity)
                )
                .ForMember(dest => dest.Note, opt => opt.MapFrom(src => src.Note));

            CreateMap<Return, ReturnRes>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.ReturnDetails));
            CreateMap<ReturnDetail, ReturnItemRes>()
                .ForMember(
                    dest => dest.DamagedQuantity,
                    opt => opt.MapFrom(src => src.DamagedQuantity)
                )
                .ForMember(
                    dest => dest.ReturnedQuantity,
                    opt => opt.MapFrom(src => src.ReturnedQuantity)
                )
                .ForMember(dest => dest.Note, opt => opt.MapFrom(src => src.Note));

            CreateMap<InventorySnapshot, InventorySnapshotRes>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.SnapshotDetails))
                .ForMember(
                    dest => dest.WarehouseName,
                    opt => opt.MapFrom(src => src.Warehouse.Name)
                );
            CreateMap<SnapshotDetail, InventorySnapshotItemRes>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name));
        }
    }
}
