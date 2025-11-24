using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLVPP.Models
{
    [Table("Department")]
    public class Department : AuditableEntity
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string? Note { get; set; }
        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
        public ICollection<StockOut> StockOuts { get; set; } = new List<StockOut>();
        public ICollection<Return> Returns { get; set; } = new List<Return>();
        public ICollection<Requisition> Requisitions { get; set; } = new List<Requisition>();

        public virtual ICollection<ApprovalInstance> ApprovalInstances { get; set; } =
            new List<ApprovalInstance>();
        public virtual ICollection<ApprovalStepInstance> ApprovalStepInstances { get; set; } =
            new List<ApprovalStepInstance>();
    }
}
