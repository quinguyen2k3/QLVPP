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

        public long? PositionId { get; set; }

        [ForeignKey("PositionId")]
        public Position? Position { get; set; }

        public long? RoleId { get; set; }

        [ForeignKey(nameof(RoleId))]
        public Role? Role { get; set; }

        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

        public ICollection<StockOut> DeliveriesRequested { get; set; } = new List<StockOut>();
        public ICollection<StockOut> DeliveriesApproved { get; set; } = new List<StockOut>();
        public ICollection<StockOut> DeliveriesReceived { get; set; } = new List<StockOut>();

        public ICollection<StockIn> StockInsRequested { get; set; } = new List<StockIn>();
        public ICollection<StockIn> StockInsApproved { get; set; } = new List<StockIn>();
        public ICollection<StockTake> StockTakesRequested { get; set; } = new List<StockTake>();
        public ICollection<StockTake> StockTakesApproved { get; set; } = new List<StockTake>();
    }
}
