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
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Requester.Name))
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

            CreateMap<StockInReq, StockIn>()
                .ForMember(dest => dest.StockInDetails, opt => opt.MapFrom(src => src.Items))
                .ForMember(dest => dest.Status, opt => opt.Ignore());
            CreateMap<StockInItemReq, StockInDetail>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice));

            CreateMap<StockIn, StockInRes>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.StockInDetails));
            CreateMap<StockInDetail, StockInItemRes>()
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice));

            CreateMap<InvalidToken, InvalidTokenRes>();

            CreateMap<StockOutReq, StockOut>()
                .ForMember(dest => dest.Status, otp => otp.Ignore())
                .ForMember(dest => dest.StockOutDetails, opt => opt.MapFrom(src => src.Items));
            CreateMap<StockOutReqItem, StockOutDetail>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity));
            CreateMap<StockOut, StockOutRes>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.StockOutDetails));
            CreateMap<StockOutDetail, StockOutResItem>()
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

            CreateMap<TransferReq, Transfer>()
                .ForMember(dest => dest.Status, otp => otp.Ignore())
                .ForMember(dest => dest.TransferDetail, opt => opt.MapFrom(src => src.Items));
            CreateMap<TransferReqDetail, StockOutDetail>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
            CreateMap<Transfer, TransferRes>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.TransferDetail));
            CreateMap<TransferDetail, TransferResDetail>();

            CreateMap<StockTakeReq, StockTake>()
                .ForMember(dest => dest.PerformedById, opt => opt.MapFrom(src => src.PerformanceId))
                .ForMember(dest => dest.Details, opt => opt.MapFrom(src => src.Items))
                .ForMember(dest => dest.WarehouseId, opt => opt.Ignore());
            CreateMap<StockTakeDetail, StockTakeReqItem>();

            CreateMap<StockTake, StockTakeRes>()
                .ForMember(
                    dest => dest.PerformanceId,
                    opt => opt.MapFrom(src => src.PerformedById)
                );
            CreateMap<StockTakeDetail, StockTakeResItem>()
                .ForMember(dest => dest.Difference, opt => opt.MapFrom(src => src.Difference));
        }
    }
}
