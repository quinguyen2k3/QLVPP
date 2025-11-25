using System.ComponentModel.DataAnnotations;

namespace QLVPP.DTOs.Request
{
    public class ApproveReq
    {
        [Required(ErrorMessage = "RequisitionId is required")]
        public long RequisitionId { get; set; }

        [StringLength(1000, ErrorMessage = "Comments cannot exceed 1000 characters")]
        public string? Comments { get; set; }
    }
}
