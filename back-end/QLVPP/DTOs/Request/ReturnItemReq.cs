using System.ComponentModel.DataAnnotations;

namespace QLVPP.DTOs.Request
{
    public class ReturnItemReq
    {
        [Required(ErrorMessage = "ProductId is required")]
        public long ProductId { set; get; }

        [Required(ErrorMessage = "ReturnQuantity is required")]
        public int ReturnedQuantity { set; get; }

        [Required(ErrorMessage = "DamagedQuantity is required")]
        public int DamagedQuantity { set; get; }
        public string? Note { set; get; }
    }
}
