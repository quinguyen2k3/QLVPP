using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLVPP.Models
{
    [Table("Delivery")]
    public class Delivery : AuditableEntity
    {
        public DateOnly DeliveryDate { get; set; }

        [Required]
        public long DepartmentId { get; set; }

        [ForeignKey(nameof(DepartmentId))]
        public Department Department { get; set; } = null!;

        [Required]
        public long WarehouseId { get; set; }

        [ForeignKey(nameof(WarehouseId))]
        public Warehouse Warehouse { get; set; } = null!;
        public string Status { get; set; } = string.Empty;
        public ICollection<DeliveryDetail> DeliveryDetails { get; set; } =
            new List<DeliveryDetail>();
        public ICollection<Return> Returns { get; set; } = new List<Return>();
    }
}
