using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLVPP.Models
{
    [Table("AssetLoan")]
    public class AssetLoan
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [Required]
        public long DeliveryDetailId { get; set; } 

        [ForeignKey(nameof(DeliveryDetailId))]
        public DeliveryDetail DeliveryDetail { get; set; } = null!;

        public int IssuedQuantity { get; set; }
        public int ReturnedQuantity { get; set; }
        public int DamagedQuantity { get; set; }
        public ICollection<ReturnDetail> ReturnDetails{ get; set; } = new List<ReturnDetail>();
    }
}