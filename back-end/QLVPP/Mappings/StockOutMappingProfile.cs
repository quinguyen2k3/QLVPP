using AutoMapper;
using QLVPP.DTOs.Request;
using QLVPP.DTOs.Response;
using QLVPP.Models;

namespace QLVPP.Mappings
{
    public class StockOutMappingProfile : Profile
    {
        public StockOutMappingProfile()
        {
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
        }
    }
}
