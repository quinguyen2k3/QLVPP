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
        public long StepId { get; set; }

        [ForeignKey(nameof(StepId))]
        public ApprovalStep Step { get; set; } = null!;

        [Required]
        public int StepOrder { get; set; }

        [Required]
        public long AssignedToId { get; set; }

        [ForeignKey(nameof(AssignedToId))]
        public Employee AssignedTo { get; set; } = null!;

        [StringLength(20)]
        public string ApprovalType { get; set; } = "SEQUENTIAL";

        public int SequenceInGroup { get; set; } = 1;

        public bool IsMandatory { get; set; } = true;

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "WAITING";
        public long? ApprovedById { get; set; }

        [ForeignKey(nameof(ApprovedById))]
        public Employee? ApprovedBy { get; set; }

        public DateTime? ApprovedDate { get; set; }

        [StringLength(20)]
        public string? Action { get; set; }

        [StringLength(1000)]
        public string? Comments { get; set; }
    }
}
