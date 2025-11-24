using AutoMapper;
using QLVPP.DTOs.Request;
using QLVPP.DTOs.Response;
using QLVPP.Models;

namespace QLVPP.Mappings
{
    public class RequisitionMappingProfile : Profile
    {
        public RequisitionMappingProfile()
        {
            CreateMap<RequisitionReq, Requisition>()
                .ForMember(dest => dest.RequisitionDetails, opt => opt.MapFrom(src => src.Items));

            CreateMap<RequisitionItemReq, RequisitionDetail>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<Requisition, RequisitionRes>()
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Requester.Name))
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.RequisitionDetails));
            CreateMap<RequisitionDetail, RequisitionItemRes>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.UnitName, opt => opt.MapFrom(src => src.Product.Unit.Name));
        }
    }
}
