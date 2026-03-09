using System.ComponentModel.DataAnnotations.Schema;

namespace QLVPP.Models
{
    [Table("ApprovalLog")]
    public class ApprovalLog : BaseEntity
    {
        public long MaterialRequestId { get; set; }
        public long ActorId { get; set; }
        public string Action { get; set; } = string.Empty;
        public long? ToUserId { get; set; }
        public string Step { get; set; } = string.Empty;
        public string? Comment { get; set; }
        public DateTime LogTime { get; set; }

        [ForeignKey(nameof(MaterialRequestId))]
        public MaterialRequest MaterialRequest { get; set; } = null!;
    }
}
