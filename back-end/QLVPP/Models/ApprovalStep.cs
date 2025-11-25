using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLVPP.Models
{
    public class ApprovalStep : BaseEntity
    {
        [Required]
        public long TemplateId { get; set; }

        [ForeignKey(nameof(TemplateId))]
        public ApprovalTemplate Template { get; set; } = null!;

        [Required]
        public int StepOrder { get; set; }

        [StringLength(200)]
        public string? StepName { get; set; }

        [Required]
        [StringLength(20)]
        public string ApprovalType { get; set; } = "SEQUENTIAL";
        public int? RequiredApprovals { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        public virtual ICollection<ApprovalStepApprover> Approvers { get; set; } =
            new List<ApprovalStepApprover>();
    }
}
