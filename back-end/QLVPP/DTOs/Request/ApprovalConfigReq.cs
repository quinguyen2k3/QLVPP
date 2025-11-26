using System.ComponentModel.DataAnnotations;

namespace QLVPP.DTOs.Request
{
    public class ApprovalConfigReq
    {
        [Required(ErrorMessage = "Approval type is required")]
        [RegularExpression(
            "SEQUENTIAL|PARALLEL",
            ErrorMessage = "Approval type must be SEQUENTIAL or PARALLEL"
        )]
        public string ApprovalType { get; set; } = "SEQUENTIAL";

        [Range(1, 100, ErrorMessage = "Number of required approvals must be between 1 and 100")]
        public int? RequiredApprovals { get; set; }

        [Required(ErrorMessage = "At least one approver is required")]
        [MinLength(1, ErrorMessage = "At least one approver is required")]
        public List<ApproverReq> Approvers { get; set; } = new();
    }
}
