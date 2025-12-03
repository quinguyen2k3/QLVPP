using AutoMapper;
using QLVPP.Constants.Status;
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
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.RequesterId, opt => opt.Ignore())
                .ForMember(dest => dest.Requester, opt => opt.Ignore())
                .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Note))
                .ForMember(
                    dest => dest.Status,
                    opt => opt.MapFrom(src => RequisitionStatus.Pending)
                )
                .ForMember(dest => dest.RequestDate, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.DepartmentId, opt => opt.MapFrom(src => src.DepartmentId))
                .ForMember(dest => dest.Department, opt => opt.Ignore())
                .ForMember(dest => dest.RequisitionDetails, opt => opt.MapFrom(src => src.Items))
                .ForMember(dest => dest.Config, opt => opt.MapFrom(src => src.Config))
                .ForMember(dest => dest.IsActivated, opt => opt.MapFrom(src => src.IsActivated));

            CreateMap<RequisitionItemReq, RequisitionDetail>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.RequisitionId, opt => opt.Ignore())
                .ForMember(dest => dest.Requisition, opt => opt.Ignore())
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
                .ForMember(dest => dest.Product, opt => opt.Ignore())
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity));

            CreateMap<Requisition, RequisitionRes>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(
                    dest => dest.Code,
                    opt => opt.MapFrom(src => GenerateCode(src.Id, src.CreatedDate))
                )
                .ForMember(dest => dest.RequesterId, opt => opt.MapFrom(src => src.RequesterId))
                .ForMember(dest => dest.DepartmentId, opt => opt.MapFrom(src => src.DepartmentId))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.Note, opt => opt.MapFrom(src => src.Notes))
                .ForMember(dest => dest.IsActivated, opt => opt.MapFrom(src => src.IsActivated))
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.RequisitionDetails))
                .ForMember(dest => dest.Config, opt => opt.MapFrom(src => src.Config));
            CreateMap<RequisitionDetail, RequisitionItemRes>()
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(
                    dest => dest.UnitName,
                    opt =>
                        opt.MapFrom(src => src.Product.Unit != null ? src.Product.Unit.Name : null)
                )
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity));
        }

        #region Helper Methods

        private static string GenerateCode(long id, DateTime createdDate)
        {
            return $"REQ{createdDate:yyMMdd}{id:D4}";
        }
        #endregion
    }
}
