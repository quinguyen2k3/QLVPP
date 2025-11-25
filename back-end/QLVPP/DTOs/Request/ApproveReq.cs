using System.ComponentModel.DataAnnotations;

namespace QLVPP.DTOs.Request
{
    public class ApproveReq
    {
        [Required]
        public long StepInstanceId { get; set; }

        [StringLength(1000, ErrorMessage = "Comments cannot exceed 1000 characters")]
        public string? Comments { get; set; }
    }
}
