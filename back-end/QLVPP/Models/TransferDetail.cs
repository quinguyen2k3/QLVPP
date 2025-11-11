using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLVPP.Models
{
    public class TransferDetail : BaseEntity
    {
        [Required]
        public long TransferId { get; set; }

        [ForeignKey("TransferId")]
        public Transfer Transfer { get; set; } = null!;

        [Required]
        public long ProductId { get; set; }

        [ForeignKey("ProductId")]
        public Product Product { get; set; } = null!;

        [Required]
        public int Quantity { get; set; }
    }
}
