using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLVPP.Models
{
    public class DeliveryDetail : BaseEntity
    {
        public int Quantity { get; set; }

        [Required]
        public long DeliveryId { get; set; }

        [ForeignKey(nameof(DeliveryId))]
        public Delivery Delivery { get; set; } = null!;

        [Required]
        public long ProductId { get; set; }

        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; } = null!;
    }
}
