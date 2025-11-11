using System.ComponentModel.DataAnnotations;

namespace QLVPP.DTOs.Request
{
    public class StockInItemReq
    {
        [Required(ErrorMessage = "Product Id is required")]
        public long ProductId { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "UnitPrice is required")]
        public int UnitPrice { get; set; }
    }
}
