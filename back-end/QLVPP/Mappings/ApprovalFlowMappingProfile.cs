using AutoMapper;
using QLVPP.DTOs.Request;
using QLVPP.Models;

namespace QLVPP.Mappings
{
    public class ApprovalFlowMappingProfile : Profile
    {
        public ApprovalFlowMappingProfile()
        {
            #region REQUEST -> ENTITY

            // ApprovalTemplateReq -> ApprovalTemplate
            CreateMap<ApprovalTemplateReq, ApprovalTemplate>()
                .ForMember(dest => dest.Steps, opt => opt.MapFrom(src => src.Steps))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore())
                .ForMember(dest => dest.Instances, opt => opt.Ignore());

            // ApprovalStepReq -> ApprovalTemplateStep
            CreateMap<ApprovalStepReq, ApprovalStep>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.TemplateId, opt => opt.Ignore())
                .ForMember(dest => dest.Template, opt => opt.Ignore())
                .ForMember(dest => dest.Position, opt => opt.Ignore());

            #endregion

            #region ENTITY -> RESPONSE
            CreateMap<ApprovalTemplate, ApprovalTemplateRes>()
                .ForMember(
                    dest => dest.NoteTypeDisplay,
                    opt => opt.MapFrom(src => GetNoteTypeDisplay(src.NoteType))
                )
                .ForMember(
                    dest => dest.Steps,
                    opt => opt.MapFrom(src => src.Steps.OrderBy(s => s.StepOrder))
                );

            // ApprovalTemplateStep -> ApprovalStepRes
            CreateMap<ApprovalStep, ApprovalStepRes>()
                .ForMember(
                    dest => dest.PositionName,
                    opt => opt.MapFrom(src => src.Position != null ? src.Position.Name : "")
                )
                .ForMember(
                    dest => dest.ScopeDisplay,
                    opt => opt.MapFrom(src => GetScopeDisplay(src.Scope))
                );
            #endregion
        }

        #region HELPER METHODS

        private static string GetNoteTypeDisplay(string noteType)
        {
            return noteType switch
            {
                "REQUISITION" => "Phiếu yêu cầu",
                "STOCK_OUT" => "Phiếu xuất kho",
                "STOCK_IN" => "Phiếu nhập kho",
                "TRANSFER" => "Phiếu chuyển kho",
                "PURCHASE_ORDER" => "Đơn mua hàng",
                _ => noteType,
            };
        }

        private static string GetScopeDisplay(string scope)
        {
            return scope switch
            {
                "DEPARTMENT" => "Phòng ban",
                "COMPANY" => "Toàn công ty",
                "BRANCH" => "Chi nhánh",
                _ => scope,
            };
        }

        private static string GetStatusDisplay(string status)
        {
            return status switch
            {
                "PENDING" => "Đang chờ duyệt",
                "APPROVED" => "Đã duyệt",
                "REJECTED" => "Đã từ chối",
                "CANCELLED" => "Đã hủy",
                _ => status,
            };
        }

        private static string GetStepStatusDisplay(string status)
        {
            return status switch
            {
                "WAITING" => "Chờ duyệt",
                "APPROVED" => "Đã duyệt",
                "REJECTED" => "Đã từ chối",
                "SKIPPED" => "Bỏ qua",
                _ => status,
            };
        }

        #endregion
    }
}
