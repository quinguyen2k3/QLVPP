using AutoMapper;
using QLVPP.DTOs.Request;
using QLVPP.DTOs.Response;
using QLVPP.Models;

namespace QLVPP.Mappings
{
    public class StockInMappingProfile : Profile
    {
        public StockInMappingProfile()
        {
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
        }
    }
}
