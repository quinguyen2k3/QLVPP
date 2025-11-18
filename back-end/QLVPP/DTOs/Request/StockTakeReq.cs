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
        [Range(1, long.MaxValue, ErrorMessage = "Employee ID must be greater than 0.")]
        public long PerformanceId { get; set; }

        public List<StockTakeReqItem> Items { get; set; } = null!;
    }
}
