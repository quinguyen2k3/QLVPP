using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLVPP.Models
{
    [Table("Return")]
    public class Return : AuditableEntity
    {
        [Required]
        public long DepartmentId { get; set; }

        [ForeignKey(nameof(DepartmentId))]
        public Department Department { get; set; } = null!;

        [Required]
        public long WarehouseId { get; set; }

        [ForeignKey(nameof(WarehouseId))]
        public Warehouse Warehouse { get; set; } = null!;

        [Required]
        public long DeliveryId { get; set; }

        [ForeignKey(nameof(DeliveryId))]
        public Delivery Delivery { get; set; } = null!;

        [Required]
        public DateOnly ReturnDate { get; set; }

        [Required]
        public string Status { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string? Note { get; set; }
        public ICollection<ReturnDetail> ReturnDetails { get; set; } = new List<ReturnDetail>();
    }
}
