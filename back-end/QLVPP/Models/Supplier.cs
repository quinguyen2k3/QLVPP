using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLVPP.Models
{
    [Table("Supplier")]
    public class Supplier : AuditableEntity
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string? Note { get; set; }

        [Required]
        [StringLength(15)]
        public string Phone { get; set; } = string.Empty;

        [StringLength(50)]
        public string? Email { get; set; }

        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
