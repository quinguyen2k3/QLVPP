using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using QLVPP.Constants.Status;

namespace QLVPP.Models
{
    [Table("StockIn")]
    public class StockIn : AuditableEntity
    {
        [Required]
        public DateOnly StockInDate { get; set; }
        public string Status { get; set; } = RequisitionStatus.Pending;

        [Required]
        public long SupplierId { get; set; }

        [ForeignKey(nameof(SupplierId))]
        public Supplier Supplier { get; set; } = null!;

        [Required]
        public long WarehouseId { get; set; }

        [ForeignKey(nameof(WarehouseId))]
        public Warehouse Warehouse { get; set; } = null!;

        public long? RequesterId { get; set; }

        [ForeignKey("RequesterId")]
        public Employee? Requester { get; set; }

        public long? ApproverId { get; set; }

        [ForeignKey("ApproverId")]
        public Employee? Approver { get; set; }

        public DateTime? ApprovedDate { get; set; }
        public ICollection<StockInDetail> StockInDetails { get; set; } = new List<StockInDetail>();
    }
}
