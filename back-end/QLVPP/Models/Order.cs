using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using QLVPP.Constants;

namespace QLVPP.Models
{
    [Table("Order")]
    public class Order : AuditableEntity
    {
        [Required]
        public DateOnly OrderDate { get; set; }

        [Required]
        public DateOnly ExpectedDate { get; set; }
        public DateOnly? ActualDate { get; set; }
        public string Status { get; set; } = RequisitionStatus.Pending;

        [Required]
        public long SupplierId { get; set; }

        [ForeignKey(nameof(SupplierId))]
        public Supplier Supplier { get; set; } = null!;

        [Required]
        public long WarehouseId { get; set; }

        [ForeignKey(nameof(WarehouseId))]
        public Warehouse Warehouse { get; set; } = null!;
        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}
