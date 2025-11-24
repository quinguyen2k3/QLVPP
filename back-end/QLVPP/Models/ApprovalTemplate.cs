using System.ComponentModel.DataAnnotations;

namespace QLVPP.Models
{
    public class ApprovalTemplate : AuditableEntity
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(50)]
        public string? Code { get; set; }

        [Required]
        [StringLength(50)]
        public string NoteType { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        public bool IsDefault { get; set; } = false;

        public virtual ICollection<ApprovalStep> Steps { get; set; } = new List<ApprovalStep>();
        public virtual ICollection<ApprovalInstance> Instances { get; set; } =
            new List<ApprovalInstance>();
    }
}
