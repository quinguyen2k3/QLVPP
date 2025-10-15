namespace QLVPP.DTOs.Response
{
    public class DeliveryRes
    {
        public long Id { get; set; }
        public string? Note { get; set; }
        public long DepartmentId { get; set; }
        public long WarehouseId { get; set; }
        public DateOnly DeliveryDate { get; set; }
        public List<DeliveryItemRes> Items { get; set; } = new();
    }
}
