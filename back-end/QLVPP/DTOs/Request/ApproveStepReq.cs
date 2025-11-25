using System.ComponentModel.DataAnnotations;

namespace QLVPP.DTOs.Request
{
    public class ApproveStepReq
    {
        [Required]
        public long StepInstanceId { get; set; }

        [StringLength(1000, ErrorMessage = "Ghi chú không quá 1000 ký tự")]
        public string? Comments { get; set; }
    }
}
