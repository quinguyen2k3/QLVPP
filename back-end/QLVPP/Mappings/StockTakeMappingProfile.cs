using AutoMapper;
using QLVPP.DTOs.Request;
using QLVPP.DTOs.Response;
using QLVPP.Models;

public class StockTakeMappingProfile : Profile
{
    public StockTakeMappingProfile()
    {
        CreateMap<StockTakeReq, StockTake>()
            .ForMember(dest => dest.RequesterId, opt => opt.MapFrom(src => src.RequesterId))
            .ForMember(dest => dest.Details, opt => opt.MapFrom(src => src.Items))
            .ForMember(dest => dest.WarehouseId, opt => opt.Ignore());

        CreateMap<StockTakeReqItem, StockTakeDetail>()
            .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
            .ForMember(dest => dest.ActualQty, opt => opt.MapFrom(src => src.ActualQty));

        CreateMap<StockTake, StockTakeRes>()
            .ForMember(dest => dest.RequesterId, opt => opt.MapFrom(src => src.RequesterId))
            .ForMember(dest => dest.WarehouseId, opt => opt.MapFrom(src => src.WarehouseId))
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Details));

        CreateMap<StockTakeDetail, StockTakeResItem>()
            .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
            .ForMember(dest => dest.SysQty, opt => opt.MapFrom(src => src.SysQty))
            .ForMember(dest => dest.ActualQty, opt => opt.MapFrom(src => src.ActualQty));
    }
}
