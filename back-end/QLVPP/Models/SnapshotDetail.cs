using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLVPP.Models
{
    public class SnapshotDetail : BaseEntity
    {
        [Required]
        public long SnapshotId { get; set; }

        [Required]
        public long ProductId { get; set; }
        public int OpeningQty { get; set; }
        public int TotalIn { get; set; }
        public int TotalOut { get; set; }
        public int ClosingQty { get; set; }

        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; } = null!;

        [ForeignKey(nameof(SnapshotId))]
        public InventorySnapshot InventorySnapshot { get; set; } = null!;
    }
}
