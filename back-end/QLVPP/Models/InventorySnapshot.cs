using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLVPP.Models
{
    public class InventorySnapshot : BaseEntity
    {
        [Required]
        public DateOnly SnapshotDate { get; set; }

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
