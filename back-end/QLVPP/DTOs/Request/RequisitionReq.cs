using System.ComponentModel.DataAnnotations;

namespace QLVPP.DTOs.Request
{
    public class RequisitionReq
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; } = string.Empty;
        public string? Note { get; set; }

        [Required(ErrorMessage = "IsActivated status is required")]
        public bool IsActivated { get; set; }

        [Required(ErrorMessage = "IsActivated status is required")]
        public long DepartmentId { get; set; }

        [Required(ErrorMessage = "Approval config is required")]
        public ApprovalConfigReq Config { get; set; } = new();

        [Required(ErrorMessage = "At least one item is required")]
        [MinLength(1, ErrorMessage = "At least one item is required")]
        public List<RequisitionItemReq> Items { get; set; } = new();
    }
}
