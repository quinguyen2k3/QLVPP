using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLVPP.Models
{
    [Table("ReturnDetail")]
    public class ReturnDetail : BaseEntity
    {
        public int ReturnedQuantity { get; set; }
        public int DamagedQuantity { get; set; }

        [StringLength(200, ErrorMessage = "Note cannot be longer than 200 characters.")]
        public string? Note { get; set; }

        [Required]
        public long ReturnId { get; set; }

        [ForeignKey(nameof(ReturnId))]
        public Return Return { get; set; } = null!;

        [Required]
        public long ProductId { get; set; }

        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; } = null!;
    }
}
