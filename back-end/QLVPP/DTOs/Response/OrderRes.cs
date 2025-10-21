using QLVPP.DTOs.Request;

namespace QLVPP.DTOs.Response
{
    public class OrderRes
    {
        public long Id { get; set; }
        public DateOnly OrderDate { get; set; }
        public DateOnly ExpectedDate { get; set; }
        public DateOnly? ActualDate { get; set; }
        public long SupplierId { get; set; }
        public long WarehouseId { get; set; }
        public bool IsActivated { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public List<OrderItemRes> Items { get; set; } = new();
    }
}
