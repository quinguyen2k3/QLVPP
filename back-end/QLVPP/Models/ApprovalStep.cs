using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using QLVPP.Constants.Status;

namespace QLVPP.Models
{
    public class ApprovalStep : BaseEntity
    {
        [Required]
        public long RequisitionId { get; set; }

        [ForeignKey("RequisitionId")]
        public Requisition Requisition { get; set; } = null!;

        [Required]
        public int StepOrder { get; set; }

        [Required]
        public long AssignedToId { get; set; }

        [ForeignKey("AssignedToId")]
        public Employee AssignedTo { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = RequisitionStatus.Pending;

        public DateTime? ApprovedAt { get; set; }
    }
}
