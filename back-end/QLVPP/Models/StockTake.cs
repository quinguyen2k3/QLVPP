using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using QLVPP.Constants.Status;

namespace QLVPP.Models
{
    public class StockTake : AuditableEntity
    {
        [Required]
        [MaxLength(20)]
        public required string Code { get; set; }

        [Required]
        [MaxLength(500)]
        public required string Purpose { get; set; }

        [Required]
        public long WarehouseId { get; set; }

        [ForeignKey("WarehouseId")]
        public required Warehouse Warehouse { get; set; }

        public long? RequesterId { get; set; }
        public Employee? Requester { get; set; }

        public long? ApproverId { get; set; }
        public Employee? Approver { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string ReferenceId { get; set; } = string.Empty;
        public string Status { get; set; } = StockTakeStatus.Pending;

        public required List<StockTakeDetail> Details { get; set; }

        public StockTake()
        {
            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            string sequence = (timestamp % 100000000).ToString("D8");

            Code = $"PKK-{sequence}";
        }
    }
}
