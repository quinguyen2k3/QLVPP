namespace QLVPP.DTOs.Response
{
    public class StockInRes
    {
        public long Id { get; set; }
        public DateOnly StockInDate { get; set; }
        public long SupplierId { get; set; }

        public long WarehouseId { get; set; }
        public bool IsActivated { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public List<StockInItemRes> Items { get; set; } = new();
    }
}
