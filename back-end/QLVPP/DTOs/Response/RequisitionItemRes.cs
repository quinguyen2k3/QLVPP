namespace QLVPP.DTOs.Response
{
    public class RequisitionItemRes
    {
        public long ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string UnitName { get; set; } = string.Empty;
        public string Purpose { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }
}
