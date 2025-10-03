namespace QLVPP.DTOs.Response
{
    public class DeliveryItemRes
    {
        public long ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string UnitName { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }
}
