using AutoMapper;
using QLVPP.DTOs.Response;
using QLVPP.Models;

namespace QLVPP.Mappings // Hoặc namespace của bạn
{
    public class InventorySnapshotMappingProfile : Profile
    {
        public InventorySnapshotMappingProfile()
        {
            CreateMap<SnapshotDetail, InventorySnapshotItemRes>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.UnitName, opt => opt.MapFrom(src => src.Product.Unit.Name));

            CreateMap<InventorySnapshot, InventorySnapshotRes>()
                .ForMember(
                    dest => dest.WarehouseName,
                    opt => opt.MapFrom(src => src.Warehouse.Name)
                )
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.SnapshotDetails));
        }
    }
}
