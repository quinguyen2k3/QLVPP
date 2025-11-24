using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLVPP.Models
{
    public class ApprovalStepInstance : AuditableEntity
    {
        [Required]
        public long ApprovalInstanceId { get; set; }

        [ForeignKey(nameof(ApprovalInstanceId))]
        public ApprovalInstance ApprovalInstance { get; set; } = null!;

        [Required]
        public long TemplateStepId { get; set; }

        [ForeignKey(nameof(TemplateStepId))]
        public ApprovalStep TemplateStep { get; set; } = null!;

        [Required]
        public int StepOrder { get; set; }

        [Required]
        public long PositionId { get; set; }

        [ForeignKey(nameof(PositionId))]
        public Position Position { get; set; } = null!;

        public long? AssignedToId { get; set; }

        [ForeignKey(nameof(AssignedToId))]
        public Employee? AssignedTo { get; set; }

        public long? AssignedDepartmentId { get; set; }

        [ForeignKey(nameof(AssignedDepartmentId))]
        public Department? AssignedDepartment { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "WAITING";

        public long? ApprovedById { get; set; }

        [ForeignKey(nameof(ApprovedById))]
        public Employee? ApprovedBy { get; set; }

        public DateTime? ApprovedDate { get; set; }

        [StringLength(1000)]
        public string? Comments { get; set; }
    }
}
