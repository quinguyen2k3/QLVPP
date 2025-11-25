using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLVPP.Models
{
    public class ApprovalStepApprover : BaseEntity
    {
        [Required]
        public long TemplateStepId { get; set; }

        [ForeignKey(nameof(TemplateStepId))]
        public ApprovalStep TemplateStep { get; set; } = null!;

        [Required]
        public long EmployeeId { get; set; }

        [ForeignKey(nameof(EmployeeId))]
        public Employee Employee { get; set; } = null!;
        public int Priority { get; set; } = 1;
        public bool IsMandatory { get; set; } = true;
    }
}
