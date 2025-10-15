using System.ComponentModel.DataAnnotations;

namespace QLVPP.DTOs.Request
{
    public class ReturnReq
    {
        [Required(ErrorMessage = "DepartmentId is required")]
        public long DepartmentId { get; set; }

        [Required(ErrorMessage = "WarehouseId is required")]
        public long WarehouseId { get; set; }

        [Required(ErrorMessage = "DeliveryId is required")]
        public long DeliveryId { get; set; }
        public string? Note { get; set; }
        public DateOnly ReturnDate { get; set; }
        public List<ReturnItemReq> Items { get; set; } = new List<ReturnItemReq>();
    }
}
