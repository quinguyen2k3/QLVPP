using System.ComponentModel.DataAnnotations;

namespace QLVPP.DTOs.Request
{
    public class RequisitionItemReq
    {
        [Required]
        public long ProductId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public int Quantity { get; set; }
        public string? Purpose { get; set; }
    }
}
