using System.ComponentModel.DataAnnotations;

namespace QLVPP.DTOs.Request
{
    public class ApprovalStepReq
    {
        [Required]
        [Range(1, 100, ErrorMessage = "Step order must be between 1 and 100")]
        public int StepOrder { get; set; }

        [StringLength(200)]
        public string? StepName { get; set; }

        [Required(ErrorMessage = "Position is required")]
        public long PositionId { get; set; }

        [Required]
        [RegularExpression("DEPARTMENT|COMPANY|BRANCH", ErrorMessage = "Scope is not valid")]
        public string Scope { get; set; } = "DEPARTMENT";
    }
}
