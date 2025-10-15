namespace QLVPP.DTOs.Response
{
    public class ProductReportRes
    {
        public long ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }
}
