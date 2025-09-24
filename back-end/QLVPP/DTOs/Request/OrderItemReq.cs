using System.ComponentModel.DataAnnotations;

namespace QLVPP.DTOs.Request
{
    public class OrderItemReq
    {
        [Required(ErrorMessage = "Product Id is required")]
        public long ProductId { get; set; }
        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Received is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Received cannot be negative")]
        public int Received { get; set; }
        [Required(ErrorMessage = "UnitPrice is required")]
        public int UnitPrice { get; set; }
    }
}
