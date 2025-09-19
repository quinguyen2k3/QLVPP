using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLVPP.Models
{
    [Table("Employee")]
    public class Employee : BaseEntity
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(15)]
        public string? Phone { get; set; }
        [StringLength (100)]
        public string? Email { get; set; }

        [Required]
        [StringLength(25)]
        public string Account { get; set; } = string.Empty;
        [StringLength(200)]
        public string? Password { get; set; }
        public long? DepartmentId { get; set; }

        [ForeignKey(nameof(DepartmentId))]
        public Department Department { get; set; } = null!;
        public ICollection<Requisition> Requisitions { get; set; } = new List<Requisition>();
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}
