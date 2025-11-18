using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLVPP.Models
{
    public class StockTake : AuditableEntity
    {
        [Required]
        [MaxLength(500)]
        public required string Purpose { get; set; }

        [Required]
        public long WarehouseId { get; set; }

        [ForeignKey("WarehouseId")]
        public required Warehouse Warehouse { get; set; }

        [Required]
        public long PerformedById { get; set; }

        [ForeignKey("PerformedById")]
        public required Employee PerformedBy { get; set; }

        public required List<StockTakeDetail> Details { get; set; }
    }
}
