using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using QLVPP.Constants.Status;

namespace QLVPP.Models
{
    public class Requisition : AuditableEntity
    {
        [Required]
        public long RequesterId { get; set; }

        [ForeignKey("RequesterId")]
        public Employee Requester { get; set; } = null!;

        [Required]
        public long OriginalApproverId { get; set; }

        [ForeignKey("OriginalApproverId")]
        public Employee OriginalApprover { get; set; } = null!;

        [Required]
        public long CurrentApproverId { get; set; }

        [ForeignKey("CurrentApproverId")]
        public Employee CurrentApprover { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = RequisitionStatus.Pending;

        public DateTime? ApprovedDate { get; set; }

        public string? Notes { get; set; }

        public ICollection<RequisitionDetail> RequisitionDetails { get; set; } =
            new List<RequisitionDetail>();
    }
}
