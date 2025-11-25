using AutoMapper;
using QLVPP.DTOs.Request;
using QLVPP.DTOs.Response;
using QLVPP.Models;

namespace QLVPP.Mappings
{
    public class ApprovalFlowMappingProfile : Profile
    {
        public ApprovalFlowMappingProfile()
        {
            #region REQUEST -> ENTITY

            CreateMap<ApprovalStepReq, ApprovalStep>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.RequisitionId, opt => opt.Ignore()) // Set sau
                .ForMember(dest => dest.Requisition, opt => opt.Ignore())
                .ForMember(dest => dest.ApprovalType, opt => opt.MapFrom(src => src.ApprovalType))
                .ForMember(
                    dest => dest.RequiredApprovals,
                    opt => opt.MapFrom(src => src.RequiredApprovals)
                )
                .ForMember(dest => dest.Approvers, opt => opt.Ignore());

            // ApprovalStepReq -> ApprovalTemplateStep
            CreateMap<ApproverReq, ApprovalStepApprover>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.StepId, opt => opt.Ignore())
                .ForMember(dest => dest.Step, opt => opt.Ignore())
                .ForMember(dest => dest.EmployeeId, opt => opt.MapFrom(src => src.EmployeeId))
                .ForMember(dest => dest.Employee, opt => opt.Ignore())
                .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => src.Priority))
                .ForMember(dest => dest.IsMandatory, opt => opt.MapFrom(src => src.IsMandatory));

            #endregion

            #region ENTITY -> RESPONSE
            CreateMap<ApprovalStep, ApprovalStepRes>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ApprovalType, opt => opt.MapFrom(src => src.ApprovalType))
                .ForMember(
                    dest => dest.ApprovalTypeDisplay,
                    opt => opt.MapFrom(src => GetApprovalTypeDisplay(src.ApprovalType))
                )
                .ForMember(
                    dest => dest.RequiredApprovals,
                    opt => opt.MapFrom(src => src.RequiredApprovals)
                )
                .ForMember(
                    dest => dest.Approvers,
                    opt => opt.MapFrom(src => src.Approvers.OrderBy(a => a.Priority))
                );

            // ===== APPROVER RESPONSE =====
            CreateMap<ApprovalStepApprover, ApproverRes>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.EmployeeId, opt => opt.MapFrom(src => src.EmployeeId))
                .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => src.Priority))
                .ForMember(dest => dest.IsMandatory, opt => opt.MapFrom(src => src.IsMandatory));
            #endregion
        }

        #region Helper Methods
        private static string GetStatusDisplay(string status) =>
            status switch
            {
                "DRAFT" => "Nháp",
                "PENDING" => "Chờ duyệt",
                "APPROVED" => "Đã duyệt",
                "REJECTED" => "Đã từ chối",
                "CANCELLED" => "Đã hủy",
                _ => status,
            };

        private static string GetApprovalTypeDisplay(string type) =>
            type switch
            {
                "SEQUENTIAL" => "Tuần tự",
                "PARALLEL" => "Song song",
                _ => type,
            };

        private static string GetApprovalStatusDisplay(string status) =>
            status switch
            {
                "PENDING" => "Đang chờ duyệt",
                "APPROVED" => "Đã hoàn thành",
                "REJECTED" => "Đã từ chối",
                "CANCELLED" => "Đã hủy",
                _ => status,
            };

        private static string GetStepStatusDisplay(string status) =>
            status switch
            {
                "WAITING" => "Chờ duyệt",
                "APPROVED" => "Đã duyệt",
                "REJECTED" => "Đã từ chối",
                "SKIPPED" => "Bỏ qua",
                "FORWARDED" => "Đã chuyển tiếp",
                _ => status,
            };

        #endregion
    }
}
