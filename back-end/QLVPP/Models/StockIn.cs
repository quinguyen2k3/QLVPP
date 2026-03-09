using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using QLVPP.Constants.Status;
using QLVPP.Constants.Types;

namespace QLVPP.Models
{
    [Table("StockIn")]
    public class StockIn : AuditableEntity
    {
        [Required]
        [MaxLength(20)]
        public required string Code { get; set; }

        [Required]
        public StockInType Type { get; set; }

        [Required]
        public DateOnly StockInDate { get; set; }

        public string Status { get; set; } = RequisitionStatus.Pending;

        public long? SupplierId { get; set; }

        [ForeignKey(nameof(SupplierId))]
        public Supplier? Supplier { get; set; }
        public long? FromWarehouseId { get; set; }

        [ForeignKey(nameof(FromWarehouseId))]
        public Warehouse? FromWarehouse { get; set; }
        public long? FromDepartmentId { get; set; }

        [ForeignKey(nameof(FromDepartmentId))]
        public Department? FromDepartment { get; set; }

        [Required]
        public long WarehouseId { get; set; }

        [ForeignKey(nameof(WarehouseId))]
        public Warehouse Warehouse { get; set; } = null!;
        public string? ReferenceId { get; set; }
        public string? Note { get; set; }
        public long? RequesterId { get; set; }

        [ForeignKey("RequesterId")]
        public Employee? Requester { get; set; }
        public long? ApproverId { get; set; }

        [ForeignKey("ApproverId")]
        public Employee? Approver { get; set; }

        public DateTime? ApprovedDate { get; set; }

        public ICollection<StockInDetail> StockInDetails { get; set; } = new List<StockInDetail>();

        public StockIn()
        {
            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            string sequence = (timestamp % 100000000).ToString("D8");

            Code = $"PNK-{sequence}";
        }
    }
}
