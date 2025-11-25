using System.ComponentModel.DataAnnotations;

namespace QLVPP.DTOs.Request
{
    public class RejectStepReq
    {
        [Required]
        public long StepInstanceId { get; set; }

        [Required(ErrorMessage = "Reaseon is required")]
        [StringLength(1000, ErrorMessage = "Lý do không quá 1000 ký tfự")]
        public string Comments { get; set; } = string.Empty;
    }
}