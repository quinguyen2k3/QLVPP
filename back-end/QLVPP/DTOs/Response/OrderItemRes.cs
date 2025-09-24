namespace QLVPP.DTOs.Response
{
    public class OrderItemRes
    {
        public long ProductId { get; set; }
        public int Quantity { get; set; }
        public int Received { get; set; }
        public int UnitPrice { get; set; }
        public int TotalLine { get; set; }
    }
}
