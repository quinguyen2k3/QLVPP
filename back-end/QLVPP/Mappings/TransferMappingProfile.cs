using AutoMapper;
using QLVPP.DTOs.Request;
using QLVPP.DTOs.Response;
using QLVPP.Models;

namespace QLVPP.Mappings
{
    public class TransferMappingProfile : Profile
    {
        public TransferMappingProfile()
        {
            CreateMap<TransferReq, Transfer>()
                .ForMember(dest => dest.Status, otp => otp.Ignore())
                .ForMember(dest => dest.TransferDetail, opt => opt.MapFrom(src => src.Items));

            CreateMap<TransferReqDetail, StockOutDetail>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<Transfer, TransferRes>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.TransferDetail));

            CreateMap<TransferDetail, TransferResDetail>();
        }
    }
}
