using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLVPP.Models
{
    public class ApprovalConfig : BaseEntity
    {
        [Required]
        public long RequisitionId { get; set; }

        [ForeignKey(nameof(RequisitionId))]
        public Requisition Requisition { get; set; } = null!;

        [Required]
        [StringLength(20)]
        public string ApprovalType { get; set; } = "SEQUENTIAL";
        public int? RequiredApprovals { get; set; }

        public virtual ICollection<Approver> Approvers { get; set; } = new List<Approver>();
    }
}
