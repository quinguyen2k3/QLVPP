using System.ComponentModel.DataAnnotations;
using QLVPP.Constants.Types;

namespace QLVPP.DTOs.Request
{
    public class StockInReq
    {
        [Required(ErrorMessage = "Stock In Date is required")]
        public DateOnly StockInDate { get; set; }

        [Required(ErrorMessage = "Stock In Type is required")]
        public StockInType Type { get; set; }

        [Required(ErrorMessage = "Destination Warehouse is required")]
        public long WarehouseId { get; set; }
        public long? SupplierId { get; set; }
        public long? FromWarehouseId { get; set; }
        public long? FromDepartmentId { get; set; }
        public string? ReferenceId { get; set; }
        public string? Note { get; set; }

        [Required(ErrorMessage = "Requester Id is required")]
        public long RequesterId { get; set; }
        public List<StockInItemReq> Items { get; set; } = new();
    }
}
