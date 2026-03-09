using System.ComponentModel.DataAnnotations.Schema;
using QLVPP.Constants.Types;

namespace QLVPP.Models
{
    [Table("MaterialRequest")]
    public class MaterialRequest : AuditableEntity
    {
        public string Code { get; set; } = string.Empty;
        public RequestType Type { get; set; }
        public long RequesterId { get; set; }
        public long WarehouseId { get; set; }
        public string Status { get; set; } = string.Empty;
        public long? ApproverId { get; set; }
        public string? Purpose { get; set; }
        public DateOnly? ExpectedDate { get; set; }
        public string? ReferenceId { get; set; }

        [ForeignKey(nameof(WarehouseId))]
        public Warehouse Warehouse { get; set; } = null!;

        [ForeignKey(nameof(RequesterId))]
        public Employee Requester { get; set; } = null!;

        [ForeignKey(nameof(ApproverId))]
        public Employee Approver { get; set; } = null!;
        public ICollection<MaterialRequestDetail> Details { get; set; } =
            new List<MaterialRequestDetail>();
        public ICollection<ApprovalLog> ApprovalLogs { get; set; } = new List<ApprovalLog>();

        public void GenerateCode()
        {
            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            string sequence = (timestamp % 100000000).ToString("D8");
            string prefix = Type == RequestType.Return ? "PHTVT" : "PYCVT";

            Code = $"{prefix}-{sequence}";
        }
    }
}
