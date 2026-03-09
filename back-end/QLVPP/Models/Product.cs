using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLVPP.Models
{
    public class Product : AuditableEntity
    {
        [Required]
        [StringLength(20)]
        public string ProdCode { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(255)]
        public string? ImagePath { get; set; }
        public double? Weight { get; set; }
        public double? Width { get; set; }
        public double? Height { get; set; }
        public double? Depth { get; set; }
        public bool IsAsset { get; set; }

        [Required]
        public long UnitId { get; set; }

        [ForeignKey(nameof(UnitId))]
        public Unit Unit { get; set; } = null!;

        [Required]
        public long CategoryId { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public Category Category { get; set; } = null!;
        public ICollection<StockOutDetail> StockOutDetails { get; set; } =
            new List<StockOutDetail>();
        public ICollection<StockInDetail> OrderDetails { get; set; } = new List<StockInDetail>();
        public ICollection<SnapshotDetail> SnapshotDetails { get; set; } =
            new List<SnapshotDetail>();
        public ICollection<StockTakeDetail> StockTakeDetails { get; set; } =
            new List<StockTakeDetail>();
        public ICollection<MaterialRequestDetail> MaterialRequestDetails { get; set; } =
            new List<MaterialRequestDetail>();
        public ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();
    }
}
