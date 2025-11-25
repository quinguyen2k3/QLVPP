using System.ComponentModel.DataAnnotations;

namespace QLVPP.DTOs.Request
{
    public class ReassignStepReq
    {
        [Required(ErrorMessage = "Người được ủy quyền là bắt buộc")]
        public long NewAssigneeId { get; set; }

        [StringLength(500)]
        public string? Reason { get; set; }
    }
}