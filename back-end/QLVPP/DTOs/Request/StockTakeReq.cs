using System.ComponentModel.DataAnnotations;

namespace QLVPP.DTOs.Request
{
    public class StockTakeReq
    {
        [Required(ErrorMessage = "Please enter the purpose of the stock take.")]
        [StringLength(
            500,
            ErrorMessage = "The purpose of the stock take cannot exceed 500 characters."
        )]
        public string Purpose { get; set; } = null!;

        [Required(ErrorMessage = "Please select an employee to perform the stock take.")]
        public long RequesterId { get; set; }

        [Required(ErrorMessage = "Please select an warehouse to perform the stock take")]
        public long WarehouseId { get; set; }
        public List<StockTakeReqItem> Items { get; set; } = null!;
    }
}
