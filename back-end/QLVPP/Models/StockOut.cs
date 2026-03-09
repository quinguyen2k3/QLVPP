using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using QLVPP.Constants.Types;

namespace QLVPP.Models
{
    [Table("StockOut")]
    public class StockOut : AuditableEntity
    {
        [Required]
        [MaxLength(20)]
        public required string Code { get; set; }
        public DateOnly StockOutDate { get; set; }

        [Required]
        public StockOutType Type { get; set; }

        [Required]
        public long WarehouseId { get; set; }
        public Warehouse Warehouse { get; set; } = null!;

        public long? DepartmentId { get; set; }
        public Department? Department { get; set; }

        public long? ToWarehouseId { get; set; }
        public Warehouse? ToWarehouse { get; set; }
        public string? Note { get; set; }
        public long RequesterId { get; set; }
        public Employee Requester { get; set; }

        public long? ApproverId { get; set; }
        public Employee? Approver { get; set; }
        public DateTime? ApprovedDate { get; set; }

        public long? ReceiverId { get; set; }
        public Employee? Receiver { get; set; }
        public DateTime? ReceivedDate { get; set; }

        public string? ReferenceId { get; set; }

        public string Status { get; set; } = string.Empty;

        public ICollection<StockOutDetail> StockOutDetails { get; set; } =
            new List<StockOutDetail>();

        public StockOut()
        {
            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            string sequence = (timestamp % 100000000).ToString("D8");

            Code = $"PXK-{sequence}";
        }
    }
}
