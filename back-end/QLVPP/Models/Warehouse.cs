using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLVPP.Models
{
    [Table("Warehouse")]
    public class Warehouse : AuditableEntity
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string? Note { get; set; }
        public ICollection<StockIn> Orders { get; set; } = new List<StockIn>();
        public ICollection<StockOut> Deliveries { get; set; } = new List<StockOut>();
        public ICollection<Return> Returns { get; set; } = new List<Return>();
        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
        public ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();
        public ICollection<InventorySnapshot> InventorySnapshots { get; set; } =
            new List<InventorySnapshot>();

        public ICollection<Transfer> TransfersFrom { get; set; } = new List<Transfer>();

        public ICollection<Transfer> TransfersTo { get; set; } = new List<Transfer>();
        public ICollection<StockTake> StockTake { get; set;} = new List<StockTake>();
    }
}
