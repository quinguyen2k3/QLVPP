using QLVPP.DTOs.Request;

namespace QLVPP.DTOs.Response
{
    public class StockTakeRes
    {
        public long Id { get; set; }
        public string Purpose { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
        public long RequesterId { get; set; }
        public long WarehouseId { get; set; }
        public string Code { get; set; } = null!;
        public string Status { get; set; } = null!;
        public required List<StockTakeResItem> Items { get; set; }
    }
}
