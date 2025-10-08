using System.ComponentModel.DataAnnotations;

namespace QLVPP.DTOs.Request
{
    public class DeliveryReq
    {
        public string? Note { get; set; }
        [Required(ErrorMessage = "Delivery Date is required")]
        public DateOnly DeliveryDate { get; set; }
        [Required(ErrorMessage = "DepartmentId is required")]
        public long DepartmentId { get; set; }
        [Required(ErrorMessage = "WarehouseId is required")]
        public long WarehouseId { get; set; }
        [Required(ErrorMessage = "IsActivated status is required")]
        public bool IsActivated { get; set; }

        public List<DeliveryItemReq> Items { get; set; } = new();
    }
}
