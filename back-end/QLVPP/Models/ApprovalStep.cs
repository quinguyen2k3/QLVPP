using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLVPP.Models
{
    public class ApprovalStep : BaseEntity
    {
        [Required]
        public long TemplateId { get; set; }

        [ForeignKey(nameof(TemplateId))]
        public ApprovalTemplate Template { get; set; } = null!;

        [Required]
        public int StepOrder { get; set; }

        [StringLength(200)]
        public string? StepName { get; set; }

        [Required]
        public long PositionId { get; set; }

        [ForeignKey(nameof(PositionId))]
        public Position Position { get; set; } = null!;

        [StringLength(20)]
        public string Scope { get; set; } = "DEPARTMENT";
    }
}
