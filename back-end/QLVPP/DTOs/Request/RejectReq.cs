using System.ComponentModel.DataAnnotations;

namespace QLVPP.DTOs.Request
{
    public class RejectReq
    {
        [Required(ErrorMessage = "RequisitionId is required")]
        public long RequisitionId { get; set; }

        [Required(ErrorMessage = "Reason for rejection is required")]
        [StringLength(1000, ErrorMessage = "Reason cannot exceed 1000 characters")]
        [MinLength(10, ErrorMessage = "Reason must be at least 10 characters")]
        public string Comments { get; set; } = string.Empty;
    }
}
