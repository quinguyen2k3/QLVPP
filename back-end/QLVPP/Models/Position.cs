using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLVPP.Models
{
    public class Position : AuditableEntity
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string? Note { get; set; }

        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}
