namespace QLVPP.DTOs.Response
{
    public class UsageItemRes
    {
        public long ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public int Quantity { get; set; }
    }
}
