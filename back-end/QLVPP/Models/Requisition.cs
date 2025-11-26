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

        [StringLength(500)]
        public string? Notes { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = RequisitionStatus.Pending;

        [Required]
        public DateTime RequestDate { get; set; } = DateTime.Now;

        public long? DepartmentId { get; set; }

        [ForeignKey("DepartmentId")]
        public Department? Department { get; set; }
        public ApprovalConfig Config { get; set; } = null!;
        public ApprovalProcess Process { get; set; } = null!;
        public ICollection<RequisitionDetail> RequisitionDetails { get; set; } =
            new List<RequisitionDetail>();
    }
}
