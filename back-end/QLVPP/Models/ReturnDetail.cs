using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLVPP.Models
{
    [Table("ReturnDetail")]
    public class ReturnDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [Required]
        public long ReturnId { get; set; }
        [ForeignKey(nameof(ReturnId))]
        public Return Return { get; set; } = null!;

        [Required]
        public long AssetLoanId { get; set; }
        [ForeignKey(nameof(AssetLoanId))]
        public AssetLoan AssetLoan { get; set; } = null!;

        public int ReturnedQuantity { get; set; }
        public int DamagedQuantity { get; set; }
        [StringLength(200, ErrorMessage = "Note cannot be longer than 200 characters.")]
        public string? Note { get; set; } 

    }
}