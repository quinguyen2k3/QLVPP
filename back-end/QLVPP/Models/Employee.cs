using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLVPP.Models
{
    [Table("Employee")]
    public class Employee : AuditableEntity
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(15)]
        public string? Phone { get; set; }

        [StringLength(100)]
        public string? Email { get; set; }

        [Required]
        [StringLength(25)]
        public string Account { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Password { get; set; } = null!;

        public long? WarehouseId { get; set; }

        [ForeignKey(nameof(WarehouseId))]
        public Warehouse? Warehouse { get; set; }
        public long? DepartmentId { get; set; }

        [ForeignKey(nameof(DepartmentId))]
        public Department Department { get; set; } = null!;

        [InverseProperty("Requester")]
        public ICollection<Requisition> RequisitionsCreated { get; set; } = new List<Requisition>();

        [InverseProperty("OriginalApprover")]
        public ICollection<Requisition> RequisitionsToOriginallyApprove { get; set; } =
            new List<Requisition>();

        [InverseProperty("CurrentApprover")]
        public ICollection<Requisition> RequisitionsToCurrentlyApprove { get; set; } =
            new List<Requisition>();
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

        public ICollection<StockOut> DeliveriesRequested { get; set; } = new List<StockOut>();
        public ICollection<StockOut> DeliveriesApproved { get; set; } = new List<StockOut>();
        public ICollection<StockOut> DeliveriesReceived { get; set; } = new List<StockOut>();

        public ICollection<Transfer> TransfersRequested { get; set; } = new List<Transfer>();
        public ICollection<Transfer> TransfersApproved { get; set; } = new List<Transfer>();
        public ICollection<Transfer> TransfersReceived { get; set; } = new List<Transfer>();

        public ICollection<StockIn> StockInsRequested { get; set; } = new List<StockIn>();
        public ICollection<StockIn> StockInsApproved { get; set; } = new List<StockIn>();
    }
}
