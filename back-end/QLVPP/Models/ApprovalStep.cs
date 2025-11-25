using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLVPP.Models
{
    public class ApprovalStep : BaseEntity
    {
        [Required]
        public long RequisitionId { get; set; }

        [ForeignKey(nameof(RequisitionId))]
        public Requisition Requisition { get; set; } = null!;

        [Required]
        [StringLength(20)]
        public string ApprovalType { get; set; } = "SEQUENTIAL";
        public int? RequiredApprovals { get; set; }

        public virtual ICollection<ApprovalStepApprover> Approvers { get; set; } =
            new List<ApprovalStepApprover>();
    }
}
