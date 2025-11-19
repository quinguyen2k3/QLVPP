using AutoMapper;
using QLVPP.DTOs.Request;
using QLVPP.DTOs.Response;
using QLVPP.Models;

namespace QLVPP.Mappings
{
    public class StockTakeMappingProfile : Profile
    {
        public StockTakeMappingProfile()
        {
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
