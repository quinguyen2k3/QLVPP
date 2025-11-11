using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLVPP.Models
{
    public class Transfer : AuditableEntity
    {
        [Required]
        public long FromWarehouseId { get; set; }

        [ForeignKey("FromWarehouseId")]
        public Warehouse FromWarehouse { get; set; } = null!;

        [Required]
        public long ToWarehouseId { get; set; }

        [ForeignKey("ToWarehouseId")]
        public Warehouse ToWarehouse { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = string.Empty;
        public string? Note { get; set; }

        [Required]
        public DateOnly TransferredDate { get; set; }

        public long? RequesterId { get; set; }

        [ForeignKey("RequesterId")]
        public Employee? Requester { get; set; }

        public long? ApproverId { get; set; }

        [ForeignKey("ApproverId")]
        public Employee? Approver { get; set; }
        public DateTime? ApproveDate { get; set; }

        public long? ReceiverId { get; set; }

        [ForeignKey("ReceiverId")]
        public Employee? Receiver { get; set; }

        public ICollection<TransferDetail> TransferDetail { get; set; } =
            new List<TransferDetail>();
    }
}
