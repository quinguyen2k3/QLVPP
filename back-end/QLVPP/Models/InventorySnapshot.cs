using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLVPP.Models
{
    public class InventorySnapshot : AuditableEntity
    {
        [Required]
        public DateOnly FromDate { get; set; }

        [Required]
        public DateOnly ToDate { get; set; }

        [Required]
        public long WarehouseId { get; set; }

        [Required]
        public string Status { get; set; } = null!;

        [ForeignKey(nameof(WarehouseId))]
        public Warehouse Warehouse { get; set; } = null!;

        public ICollection<SnapshotDetail> SnapshotDetails { get; set; } =
            new List<SnapshotDetail>();
    }
}
