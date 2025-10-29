using System.ComponentModel.DataAnnotations;

namespace QLVPP.Models
{
    public abstract class AuditableEntity : BaseEntity
    {
        [Required]
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedBy { get; set; }
        public bool IsActivated { get; set; }
    }
}
