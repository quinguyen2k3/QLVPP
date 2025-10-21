using System.ComponentModel.DataAnnotations;

namespace QLVPP.DTOs.Request
{
    public class OrderReq
    {
        [Required(ErrorMessage = "Order Date is required")]
        public DateOnly OrderDate { get; set; }

        [Required(ErrorMessage = "Expected Date is required")]
        public DateOnly ExpectedDate { get; set; }
        public DateOnly? ActualDate { get; set; }

        [Required(ErrorMessage = "Supllier Id is required")]
        public long SupplierId { get; set; }

        [Required(ErrorMessage = "Warehouse Id is required")]
        public long WarehouseId { get; set; }

        [Required(ErrorMessage = "IsActivated status is required")]
        public bool IsActivated { get; set; }
        public List<OrderItemReq> Items { get; set; } = new();
    }
}
