namespace QLVPP.DTOs.Response
{
    public class StockOutRes
    {
        public long Id { get; set; }
        public string? Note { get; set; }

        public long DepartmentId { get; set; }
        public long WarehouseId { get; set; }
        public DateOnly StockOutDate { get; set; }
        public List<StockOutResItem> Items { get; set; } = new();
    }
}
