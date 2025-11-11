using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLVPP.Models
{
    [Table("StockInDetail")]
    public class StockInDetail : BaseEntity
    {
        public int Quantity { get; set; }

        [Required]
        public long StockInId { get; set; }

        [ForeignKey(nameof(StockInId))]
        public StockIn StockIn { get; set; } = null!;

        [Required]
        public long ProductId { get; set; }

        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; } = null!;

        [Required]
        public int UnitPrice { get; set; }

        public double Total => Quantity * UnitPrice;
    }
}
