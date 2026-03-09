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
            CreateMap<ProductReq, Product>().ForMember(dest => dest.ImagePath, opt => opt.Ignore());

            CreateMap<Product, ProductRes>()
                .ForMember(
                    dest => dest.Quantity,
                    opt => opt.MapFrom(src => src.Inventories.Sum(x => x.Quantity))
                )
                .ForMember(
                    dest => dest.WarehouseId,
                    opt =>
                        opt.MapFrom(src =>
                            src.Inventories.Select(x => x.WarehouseId).FirstOrDefault()
                        )
                );
        }
    }
}
