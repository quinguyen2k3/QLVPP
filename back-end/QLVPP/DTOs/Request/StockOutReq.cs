using System.ComponentModel.DataAnnotations;

namespace QLVPP.DTOs.Request
{
    public class StockOutReq
    {
        public string? Note { get; set; }

        [Required(ErrorMessage = "Delivery Date is required")]
        public DateOnly StockOutDate { get; set; }

        [Required(ErrorMessage = "DepartmentId is required")]
        public long DepartmentId { get; set; }

        [Required(ErrorMessage = "WarehouseId is required")]
        public long WarehouseId { get; set; }

        [Required(ErrorMessage = "IsActivated status is required")]
        public bool IsActivated { get; set; }

        public List<StockOutReqItem> Items { get; set; } = new();
    }
}
