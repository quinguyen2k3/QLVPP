using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLVPP.Models
{
    [Table("Return")]
    public class Return : BaseEntity
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
        public DateOnly ReturnDate { get; set; }

        [Required]
        public string Status { get; set; } = string.Empty;
        [StringLength(200, ErrorMessage = "Note cannot be longer than 200 characters.")]
        public string? Note { get; set; }
        public ICollection<ReturnDetail> ReturnDetails { get; set; } = new List<ReturnDetail>();
    }
}