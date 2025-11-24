using System.ComponentModel.DataAnnotations;

namespace QLVPP.DTOs.Request
{
    public class ApprovalTemplateReq
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(50)]
        public string? Code { get; set; }

        [Required(ErrorMessage = "Note type is required")]
        [RegularExpression(
            "REQUISITION|STOCK_OUT|STOCK_IN|TRANSFER|PURCHASE_ORDER",
            ErrorMessage = "Loại phiếu không hợp lệ"
        )]
        public string NoteType { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        public bool IsDefault { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "At least one step is required")]
        public List<ApprovalStepReq> Steps { get; set; } = new();
    }
}
