using AutoMapper;
using QLVPP.DTOs.Request;
using QLVPP.DTOs.Response;
using QLVPP.Models;

namespace QLVPP.Mappings
{
    public class ProductMappingProfile : Profile
    {
        public ProductMappingProfile()
        {
            CreateMap<ProductReq, Product>();

            CreateMap<Product, ProductRes>()
                .ForMember(
                    dest => dest.WarehouseId,
                    opt => opt.MapFrom(src => src.Inventory.WarehouseId)
                )
                .ForMember(
                    dest => dest.Quantity,
                    opt => opt.MapFrom(src => src.Inventory.Quantity)
                );
        }
    }
}
