using AutoMapper;
using QLVPP.DTOs.Request;
using QLVPP.DTOs.Response;
using QLVPP.Models;

namespace QLVPP.Mappings
{
    public class ReturnMappingProfile : Profile
    {
        public ReturnMappingProfile()
        {
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
        }
    }
}
