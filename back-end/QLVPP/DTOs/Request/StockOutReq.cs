using System.ComponentModel.DataAnnotations;
using QLVPP.Constants.Types;

namespace QLVPP.DTOs.Request
{
    public class StockOutReq
    {
        public StockOutType Type { get; set; }
        public string? Note { get; set; }

        [Required(ErrorMessage = "Delivery Date is required")]
        public DateOnly StockOutDate { get; set; }

        [Required(ErrorMessage = "WarehouseId is required")]
        public long WarehouseId { get; set; }

        [Required(ErrorMessage = "RequesterId is required")]
        public long RequesterId { get; set; }
        public long? DepartmentId { get; set; }
        public long? ToWarehouseId { get; set; }
        public string? ReferenceId { get; set; }

        [Required(ErrorMessage = "IsActivated status is required")]
        public bool IsActivated { get; set; }
        public List<StockOutReqItem> Items { get; set; } = new();
    }
}
