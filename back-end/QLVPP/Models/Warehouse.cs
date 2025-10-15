using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLVPP.Models
{
    [Table("Warehouse")]
    public class Warehouse : BaseEntity
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(200)]
        public string? Note { get; set; }
        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public ICollection<Delivery> Deliveries { get; set; } = new List<Delivery>();
        public ICollection<Return> Returns { get; set; } = new List<Return>();
        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
        public ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();
        public ICollection<InventorySnapshot> InventorySnapshots { get; set; } =
            new List<InventorySnapshot>();
    }
}
