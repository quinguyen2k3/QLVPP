using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLVPP.Models
{
    public class ApprovalInstance : BaseEntity
    {
        [Required]
        public long RequisitionId { get; set; }

        [ForeignKey(nameof(RequisitionId))]
        public Requisition Requisition { get; set; } = null!;

        public int CurrentStepOrder { get; set; } = 1;

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "PENDING";

        public DateTime? CompletedDate { get; set; }
        public ICollection<ApprovalStepInstance> StepInstances { get; set; } =
            new List<ApprovalStepInstance>();
    }
}
