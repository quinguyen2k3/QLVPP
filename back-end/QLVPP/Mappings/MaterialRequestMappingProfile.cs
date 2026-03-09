using AutoMapper;
using QLVPP.DTOs.Request;
using QLVPP.DTOs.Response;
using QLVPP.Models;

namespace QLVPP.Mappings
{
    public class MaterialRequestMappingProfile : Profile
    {
        public MaterialRequestMappingProfile()
        {
            CreateMap<MaterialRequestReq, MaterialRequest>()
                .ForMember(dest => dest.Details, opt => opt.MapFrom(src => src.Items));
            CreateMap<MaterialRequestReqItem, MaterialRequestDetail>();

            CreateMap<MaterialRequest, MaterialRequestRes>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Details));
            CreateMap<MaterialRequestDetail, MaterialRequestItemRes>();
        }
    }
}