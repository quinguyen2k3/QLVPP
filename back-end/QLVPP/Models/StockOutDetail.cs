using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLVPP.Models
{
    [Table("StockOutDetail")]
    public class StockOutDetail : BaseEntity
    {
        public int Quantity { get; set; }

        [Required]
        public long StockOutId { get; set; }

        [ForeignKey(nameof(StockOutId))]
        public StockOut StockOut { get; set; } = null!;

        [Required]
        public long ProductId { get; set; }

        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; } = null!;
    }
}
