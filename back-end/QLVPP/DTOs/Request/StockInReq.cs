using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using QLVPP.DTOs.Response;

namespace QLVPP.DTOs.Request
{
    public class StockInReq
    {
        [Required(ErrorMessage = "Order Date is required")]
        public DateOnly StockInDate { get; set; }

        [Required(ErrorMessage = "Supplier Id is required")]
        public long SupplierId { get; set; }

        [Required(ErrorMessage = "Warehouse Id is required")]
        public long WarehouseId { get; set; }

        [Required(ErrorMessage = "IsActivated status is required")]
        public bool IsActivated { get; set; }
        public List<StockInItemReq> Items { get; set; } = new();
    }
}
