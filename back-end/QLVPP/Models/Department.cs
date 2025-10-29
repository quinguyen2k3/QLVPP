using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLVPP.Models
{
    [Table("Department")]
    public class Department : AuditableEntity
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string? Note { get; set; }
        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
        public ICollection<Delivery> Deliveries { get; set; } = new List<Delivery>();
        public ICollection<Return> Returns { get; set; } = new List<Return>();
    }
}
