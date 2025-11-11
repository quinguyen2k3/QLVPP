using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using QLVPP.DTOs.Response;

namespace QLVPP.DTOs.Request
{
    public class StockOutReqItem
    {
        [Required]
        public long ProductId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public int Quantity { get; set; }
    }
}
