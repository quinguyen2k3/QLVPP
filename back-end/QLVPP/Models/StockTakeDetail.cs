using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLVPP.Models
{
    public class StockTakeDetail : BaseEntity
    {
        [Required]
        public long ProductId { get; set; }

        [ForeignKey("ProductId")]
        public required Product Product { get; set; }

        public int SysQty { get; set; }
        public int ActualQty { get; set; }
        public bool IsProcessed { get; set; } = false;
        public string? ReferenceId { get; set; }

        [NotMapped]
        public int Difference => ActualQty - SysQty;
    }
}
