using System.ComponentModel.DataAnnotations.Schema;

namespace QLVPP.Models
{
    [Table("MaterialRequestDetail")]
    public class MaterialRequestDetail : BaseEntity
    {
        public long MaterialRequestId { get; set; }
        public long ProductId { get; set; }
        public int Quantity { get; set; }

        [ForeignKey(nameof(MaterialRequestId))]
        public MaterialRequest MaterialRequest { get; set; } = null!;

        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; } = null!;
    }
}
