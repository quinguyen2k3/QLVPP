using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLVPP.Models
{
    public class ApprovalInstance : BaseEntity
    {
        [Required]
        public long TemplateId { get; set; }

        [ForeignKey(nameof(TemplateId))]
        public ApprovalTemplate Template { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string NoteType { get; set; } = string.Empty;

        [Required]
        public long NoteId { get; set; }

        [Required]
        public long RequesterId { get; set; }

        [ForeignKey(nameof(RequesterId))]
        public Employee Requester { get; set; } = null!;

        public long? RequesterDepartmentId { get; set; }

        [ForeignKey(nameof(RequesterDepartmentId))]
        public Department? RequesterDepartment { get; set; }

        public int CurrentStepOrder { get; set; } = 1;

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "PENDING"; // PENDING, APPROVED, REJECTED, CANCELLED

        public DateTime? CompletedDate { get; set; }

        // Navigation Properties
        public ICollection<ApprovalStepInstance> StepInstances { get; set; } =
            new List<ApprovalStepInstance>();
    }
}
